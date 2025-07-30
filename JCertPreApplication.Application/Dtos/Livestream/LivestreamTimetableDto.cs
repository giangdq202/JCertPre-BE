using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Livestream
{
    public class LivestreamTimetableDto
    {
        public Guid LivestreamId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public LivestreamStatus Status { get; set; }
        
        // Additional fields for timetable view
        public DateTime EndDateTime => ScheduledDateTime.AddMinutes(DurationMinutes);
        public bool IsLive => Status == LivestreamStatus.LIVE;
        
        /// <summary>
        /// Can user join this livestream? Only true if status is LIVE and user has permission
        /// </summary>
        public bool CanJoin { get; set; }
        
        /// <summary>
        /// Deprecated: Manual start is no longer supported. Livestreams start automatically.
        /// </summary>
        [Obsolete("Manual start is disabled. Livestreams start automatically 15 minutes before scheduled time.")]
        public bool CanStart { get; set; } = false;
        
        public UserRoleInCourse UserRole { get; set; }
        
        /// <summary>
        /// Helper property to determine if livestream will start soon (within 15 minutes)
        /// </summary>
        public bool StartsWithin15Minutes => Status == LivestreamStatus.SCHEDULED && 
                                           DateTime.UtcNow >= ScheduledDateTime.AddMinutes(-15);
                                           
        /// <summary>
        /// Helper property to show time until livestream starts/ends
        /// </summary>
        public string TimeStatus
        {
            get
            {
                var now = DateTime.UtcNow;
                return Status switch
                {
                    LivestreamStatus.SCHEDULED when now < ScheduledDateTime.AddMinutes(-15) => 
                        $"Starts in {(ScheduledDateTime - now).TotalMinutes:F0} minutes",
                    LivestreamStatus.SCHEDULED when now >= ScheduledDateTime.AddMinutes(-15) => 
                        "Starting soon (room available)",
                    LivestreamStatus.LIVE => 
                        $"Live - ends in {(EndDateTime - now).TotalMinutes:F0} minutes",
                    LivestreamStatus.COMPLETED => "Completed",
                    _ => "Unknown"
                };
            }
        }
    }

    public enum UserRoleInCourse
    {
        Instructor,
        Student,
        None
    }
}
