using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.API.Services
{
    public class LivestreamStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LivestreamStatusBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public LivestreamStatusBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<LivestreamStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Livestream Status Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndUpdateExpiredLivestreams();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking expired livestreams");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Livestream Status Background Service stopped");
        }

        private async Task CheckAndUpdateExpiredLivestreams()
        {
            using var scope = _serviceProvider.CreateScope();
            var livestreamRepository = scope.ServiceProvider.GetRequiredService<ILivestreamRepository>();

            try
            {
                // Get all LIVE livestreams
                var liveLivestreams = await livestreamRepository.GetAllAsync(
                    ls => ls.status == LivestreamStatus.LIVE);

                var currentTime = DateTime.UtcNow;
                var updatedCount = 0;

                foreach (var livestream in liveLivestreams)
                {
                    // Calculate end time: scheduledDateTime + durationMinutes
                    var endTime = livestream.scheduledDateTime.AddMinutes(livestream.durationMinutes);

                    // If current time is past the end time, mark as COMPLETED
                    if (currentTime > endTime)
                    {
                        livestream.status = LivestreamStatus.COMPLETED;
                        await livestreamRepository.UpdateAsync(livestream);
                        updatedCount++;

                        _logger.LogInformation(
                            "Livestream {LivestreamId} automatically marked as COMPLETED. " +
                            "Scheduled: {ScheduledTime}, Duration: {Duration} minutes, End time: {EndTime}",
                            livestream.livestreamId,
                            livestream.scheduledDateTime,
                            livestream.durationMinutes,
                            endTime);
                    }
                }

                // Save all changes to database
                if (updatedCount > 0)
                {
                    await livestreamRepository.SaveChangesAsync();
                    _logger.LogInformation("Updated {Count} expired livestreams to COMPLETED status", updatedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating expired livestreams");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Livestream Status Background Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
