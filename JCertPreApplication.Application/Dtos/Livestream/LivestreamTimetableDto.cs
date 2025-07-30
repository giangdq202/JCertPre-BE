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
        public bool CanJoin { get; set; }
        public bool CanStart { get; set; }
        public UserRoleInCourse UserRole { get; set; }
    }

    public enum UserRoleInCourse
    {
        Instructor,
        Student,
        None
    }
}
