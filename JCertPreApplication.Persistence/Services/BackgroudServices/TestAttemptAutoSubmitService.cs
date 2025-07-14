using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.TestAttempts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

public class TestAttemptAutoSubmitService : BackgroundService, ITestAttemptAutoSubmitController
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TestAttemptAutoSubmitService> _logger;
    private readonly ConcurrentDictionary<Guid, (DateTime EndTime, System.Timers.Timer Timer)> _activeAttempts;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); // Check every 10 seconds
    private readonly TimeSpan _idleTimeout = TimeSpan.FromMinutes(5);   // Stop after 5 minutes of no active attempts
    private DateTime _newestEndTime; // Track the newest endTime
    private bool _isRunning;
    private CancellationTokenSource _serviceCancellationTokenSource;

    public bool IsRunning => _isRunning;

    public TestAttemptAutoSubmitService(IServiceProvider serviceProvider, ILogger<TestAttemptAutoSubmitService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _activeAttempts = new ConcurrentDictionary<Guid, (DateTime, System.Timers.Timer)>();
        _newestEndTime = DateTime.MinValue;
        _isRunning = false;
        _serviceCancellationTokenSource = new CancellationTokenSource();
    }

    public void StartMonitoring()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _serviceCancellationTokenSource = new CancellationTokenSource(); // Reset cancellation token
            _logger.LogInformation("TestAttemptAutoSubmitService started monitoring due to new attempt");
        }
    }

    public void StopMonitoring()
    {
        if (_isRunning)
        {
            foreach (var attempt in _activeAttempts)
            {
                attempt.Value.Timer.Dispose(); // Clean up timers
            }
            _activeAttempts.Clear();
            _isRunning = false;
            _logger.LogInformation("TestAttemptAutoSubmitService stopped; cleared all active attempts and timers");
        }
    }

    public void AddAttempt(Guid attemptId, DateTime endTime)
    {
        StartMonitoring(); // Ensure service is running

        // Update newestEndTime if this endTime is later
        if (endTime > _newestEndTime)
        {
            _newestEndTime = endTime;
        }

        // Calculate time until endTime
        var now = DateTime.UtcNow;
        var timeUntilEnd = endTime > now ? endTime - now : TimeSpan.Zero;

        // Create a timer for this attempt
        var timer = new System.Timers.Timer
        {
            AutoReset = false, // Fire only once
            Interval = Math.Max(timeUntilEnd.TotalMilliseconds, 1) // Ensure non-zero interval
        };

        timer.Elapsed += async (sender, e) => await OnTimerElapsed(attemptId);
        timer.Start();

        _activeAttempts.TryAdd(attemptId, (endTime, timer));
        _logger.LogInformation("Added test attempt {AttemptId} with end time {EndTime} for monitoring", attemptId, endTime);
    }

    private async Task OnTimerElapsed(Guid attemptId)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var testAttemptService = scope.ServiceProvider.GetRequiredService<ITestAttemptService>();
            var testAttemptRepository = scope.ServiceProvider.GetRequiredService<ITestAttemptRepository>();

            // Verify attempt still exists and is in progress
            var attempt = await testAttemptRepository.GetByIdAsync(attemptId);
            if (attempt == null || attempt.status != TestAttemptStatus.InProgress)
            {
                _logger.LogWarning("Test attempt {AttemptId} not found or not in progress; removing from monitoring", attemptId);
                if (_activeAttempts.TryRemove(attemptId, out var removed))
                {
                    removed.Timer.Dispose();
                }
                return;
            }

            var submitDto = new SubmitTestAttemptDto
            {
                AttemptId = attemptId
            };

            await testAttemptService.SubmitTestAttemptAsync(submitDto);
            _logger.LogInformation("Auto-submitted expired test attempt {AttemptId} for user {UserId}", attemptId, attempt.userId);

            if (_activeAttempts.TryRemove(attemptId, out var removedAttempt))
            {
                removedAttempt.Timer.Dispose(); // Clean up the timer
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to auto-submit test attempt {AttemptId}", attemptId);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TestAttemptAutoSubmitService background task initialized");

        while (!stoppingToken.IsCancellationRequested && !_serviceCancellationTokenSource.Token.IsCancellationRequested)
        {
            if (!_isRunning)
            {
                _logger.LogDebug("Service is inactive; waiting for new attempts");
                await Task.Delay(_checkInterval, stoppingToken);
                continue;
            }

            // Service is running
            try
            {
                // Check if we should stop the service due to inactivity
                if (_activeAttempts.IsEmpty && _newestEndTime != DateTime.MinValue)
                {
                    if (DateTime.UtcNow - _newestEndTime > _idleTimeout)
                    {
                        _logger.LogInformation("No active attempts for {IdleTimeout} since newest endTime {NewestEndTime}; stopping and terminating service", _idleTimeout, _newestEndTime);
                        _serviceCancellationTokenSource.Cancel(); // Signal loop to exit
                        StopMonitoring();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestAttemptAutoSubmitService while checking idle timeout");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        StopMonitoring(); // Clean up on service shutdown or cancellation
        _logger.LogInformation("TestAttemptAutoSubmitService background task stopped");
    }
}