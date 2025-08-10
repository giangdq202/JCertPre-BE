using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JCertPreApplication.Persistence.Services.BackgroudServices
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
                    await ProcessLivestreamStatusUpdates();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing livestream status updates");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Livestream Status Background Service stopped");
        }

        private async Task ProcessLivestreamStatusUpdates()
        {
            using var scope = _serviceProvider.CreateScope();
            var livestreamRepository = scope.ServiceProvider.GetRequiredService<ILivestreamRepository>();
            var liveKitService = scope.ServiceProvider.GetRequiredService<ILiveKitService>();

            try
            {
                var currentTime = DateTime.UtcNow;
                
                // Process SCHEDULED livestreams that should become LIVE (15 minutes before start)
                await ProcessScheduledToLive(livestreamRepository, liveKitService, currentTime);
                
                // Process LIVE livestreams that should become COMPLETED (after end time)
                await ProcessLiveToCompleted(livestreamRepository, liveKitService, currentTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing livestream status updates");
                throw;
            }
        }

        private async Task ProcessScheduledToLive(
            ILivestreamRepository livestreamRepository, 
            ILiveKitService liveKitService, 
            DateTime currentTime)
        {
            // Get SCHEDULED livestreams that should start in 15 minutes or have already started
            var scheduledLivestreams = await livestreamRepository.GetAllAsync(
                ls => ls.status == LivestreamStatus.SCHEDULED && 
                      ls.scheduledDateTime.AddMinutes(-15) <= currentTime);

            var updatedCount = 0;

            foreach (var livestream in scheduledLivestreams)
            {
                try
                {
                    // Check if livestream is still valid (not past end time)
                    var endTime = livestream.scheduledDateTime.AddMinutes(livestream.durationMinutes);
                    
                    if (currentTime <= endTime)
                    {
                        // Create LiveKit room with long timeout to prevent auto-close, background service will manage room lifecycle
                        var roomName = GetRoomName(livestream.livestreamId);
                        var roomSettings = new Application.Contracts.RoomSettings
                        {
                            EmptyTimeout = TimeSpan.FromHours(24), // Set long timeout to prevent auto-close
                            DepartureTimeout = TimeSpan.FromHours(24), // Set same timeout for departure
                            MaxParticipants = 100,
                            Metadata = $"{{\"livestreamId\":\"{livestream.livestreamId}\",\"courseId\":\"{livestream.courseId}\"}}"
                        };

                        await liveKitService.CreateRoomAsync(roomName, roomSettings);

                        // Update status to LIVE
                        livestream.status = LivestreamStatus.LIVE;
                        await livestreamRepository.UpdateAsync(livestream);
                        updatedCount++;

                        _logger.LogInformation(
                            "Livestream {LivestreamId} automatically started. Room created with 24-hour empty and departure timeout and will be managed by background service.",
                            livestream.livestreamId);
                    }
                    else
                    {
                        // Livestream is already past end time, mark as COMPLETED
                        livestream.status = LivestreamStatus.COMPLETED;
                        await livestreamRepository.UpdateAsync(livestream);
                        updatedCount++;

                        _logger.LogInformation(
                            "Livestream {LivestreamId} marked as COMPLETED (already past end time)",
                            livestream.livestreamId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing livestream {LivestreamId} for SCHEDULED to LIVE transition", 
                        livestream.livestreamId);
                }
            }

            if (updatedCount > 0)
            {
                await livestreamRepository.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} livestreams from SCHEDULED to LIVE", updatedCount);
            }
        }

        private async Task ProcessLiveToCompleted(
            ILivestreamRepository livestreamRepository, 
            ILiveKitService liveKitService, 
            DateTime currentTime)
        {
            // Get all LIVE livestreams
            var liveLivestreams = await livestreamRepository.GetAllAsync(
                ls => ls.status == LivestreamStatus.LIVE);

            var updatedCount = 0;

            foreach (var livestream in liveLivestreams)
            {
                try
                {
                    // Calculate end time: scheduledDateTime + durationMinutes + 10 minutes buffer
                    var endTime = livestream.scheduledDateTime.AddMinutes(livestream.durationMinutes);
                    var endTimeWithBuffer = endTime.AddMinutes(10); // Add 10 minutes buffer before room deletion

                    // If current time is past the end time + buffer, mark as COMPLETED and delete room
                    if (currentTime > endTimeWithBuffer)
                    {
                        // Delete LiveKit room
                        var roomName = GetRoomName(livestream.livestreamId);
                        try
                        {
                            await liveKitService.DeleteRoomAsync(roomName);
                        }
                        catch (Exception roomEx)
                        {
                            _logger.LogWarning(roomEx, "Failed to delete room {RoomName} for livestream {LivestreamId}", 
                                roomName, livestream.livestreamId);
                        }

                        // Update status to COMPLETED
                        livestream.status = LivestreamStatus.COMPLETED;
                        await livestreamRepository.UpdateAsync(livestream);
                        updatedCount++;

                        _logger.LogInformation(
                            "Livestream {LivestreamId} automatically marked as COMPLETED. " +
                            "Scheduled: {ScheduledTime}, Duration: {Duration} minutes, End time: {EndTime}, " +
                            "Room deleted after 10-minute buffer at: {DeletionTime}",
                            livestream.livestreamId,
                            livestream.scheduledDateTime,
                            livestream.durationMinutes,
                            endTime,
                            endTimeWithBuffer);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing livestream {LivestreamId} for LIVE to COMPLETED transition", 
                        livestream.livestreamId);
                }
            }

            // Save all changes to database
            if (updatedCount > 0)
            {
                await livestreamRepository.SaveChangesAsync();
                _logger.LogInformation("Updated {Count} expired livestreams to COMPLETED status", updatedCount);
            }
        }

        private static string GetRoomName(Guid livestreamId)
        {
            return livestreamId.ToString();
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Livestream Status Background Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
