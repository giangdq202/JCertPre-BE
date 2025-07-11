using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.TestAttempts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class TestAttemptAutoSubmitService : BackgroundService, ITestAttemptAutoSubmitController
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TestAttemptAutoSubmitService> _logger;
    private readonly object _lock = new object();

    private bool _isActive = false;
    private DateTime _lastActivityTime = DateTime.MinValue;
    private readonly TimeSpan _inactivityTimeout = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public bool IsRunning => _isActive;

    public TestAttemptAutoSubmitService(IServiceProvider serviceProvider, ILogger<TestAttemptAutoSubmitService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void StartMonitoring()
    {
        lock (_lock)
        {
            if (!_isActive)
            {
                _isActive = true;
                _lastActivityTime = DateTime.UtcNow;
                _logger.LogInformation("Test attempt monitoring started");
            }
            else
            {
                // Reset the activity time if already running
                _lastActivityTime = DateTime.UtcNow;
                _logger.LogDebug("Test attempt monitoring activity time reset");
            }
        }
    }

    public void StopMonitoring()
    {
        lock (_lock)
        {
            _isActive = false;
            _logger.LogInformation("Test attempt monitoring stopped manually");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TestAttemptAutoSubmitService background task started (inactive)");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                bool shouldProcess = false;
                DateTime lastActivity;

                lock (_lock)
                {
                    shouldProcess = _isActive;
                    lastActivity = _lastActivityTime;
                }

                if (shouldProcess)
                {
                    // Check if we should stop due to inactivity
                    if (DateTime.UtcNow - lastActivity > _inactivityTimeout)
                    {
                        lock (_lock)
                        {
                            _isActive = false;
                            _logger.LogInformation("Test attempt monitoring stopped due to inactivity (5 minutes)");
                        }
                    }
                    else
                    {
                        // Process expired attempts
                        await ProcessExpiredAttempts();
                    }
                }
                else
                {
                    _logger.LogDebug("Service is inactive, waiting for activation");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestAttemptAutoSubmitService");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("TestAttemptAutoSubmitService background task stopped");
    }

    private async Task ProcessExpiredAttempts()
    {
        using var scope = _serviceProvider.CreateScope();
        var testAttemptService = scope.ServiceProvider.GetRequiredService<ITestAttemptService>();
        var testAttemptRepository = scope.ServiceProvider.GetRequiredService<ITestAttemptRepository>();

        var expiredAttempts = await testAttemptRepository.GetAllAsync(
            a => a.status == TestAttemptStatus.InProgress && a.endTime <= DateTime.UtcNow);

        if (!expiredAttempts.Any())
        {
            _logger.LogDebug("No expired test attempts found");
            return;
        }

        _logger.LogInformation("Found {Count} expired test attempts to process", expiredAttempts.Count);

        // Update last activity time since we found work to do
        lock (_lock)
        {
            _lastActivityTime = DateTime.UtcNow;
        }

        const int batchSize = 10;
        var batches = expiredAttempts.Chunk(batchSize);

        foreach (var batch in batches)
        {
            var tasks = batch.Select(async attempt =>
            {
                try
                {
                    var submitDto = new SubmitTestAttemptDto
                    {
                        AttemptId = attempt.attemptId
                    };

                    await testAttemptService.SubmitTestAttemptAsync(submitDto);
                    _logger.LogInformation("Auto-submitted expired test attempt {AttemptId} for user {UserId}",
                        attempt.attemptId, attempt.userId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to auto-submit test attempt {AttemptId} for user {UserId}",
                        attempt.attemptId, attempt.userId);
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}