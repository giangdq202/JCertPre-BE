using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JCertPreApplication.Persistence.Services.BackgroudServices
{
    public class CourseStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CourseStatusBackgroundService> _logger;
        //private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Chạy mỗi giờ
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(2); // Chạy mỗi 2 phút


        public CourseStatusBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<CourseStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateExpiredCoursesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating expired courses");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task UpdateExpiredCoursesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var courseRepository = scope.ServiceProvider.GetRequiredService<ICourseRepository>();

            _logger.LogInformation("Starting course status update process at {Time}", DateTime.UtcNow);

            try
            {
                // Lấy tất cả courses có status không phải Archived và đã qua endDate
                var allCourses = await courseRepository.GetAllAsync();
                var expiredCourses = allCourses
                    .Where(c => c.status != CourseStatus.Archived && 
                               DateTime.UtcNow > c.endDate)
                    .ToList();

                if (expiredCourses.Any())
                {
                    _logger.LogInformation("Found {Count} expired courses to archive", expiredCourses.Count);

                    foreach (var course in expiredCourses)
                    {
                        course.status = CourseStatus.Archived;
                        await courseRepository.UpdateAsync(course);
                        
                        _logger.LogInformation("Archived course {CourseId} - {Title} (EndDate: {EndDate})", 
                            course.courseId, course.title, course.endDate);
                    }

                    // Save changes to database
                    await courseRepository.SaveChangesAsync();
                    _logger.LogInformation("Successfully archived {Count} expired courses", expiredCourses.Count);
                }
                else
                {
                    _logger.LogInformation("No expired courses found to archive");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing expired courses");
                throw;
            }
        }
    }
}
