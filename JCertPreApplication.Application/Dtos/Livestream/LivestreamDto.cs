using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Livestream
{
    public class LivestreamDto
    {
        public Guid LivestreamId { get; set; }
        public Guid CourseId { get; set; }
        public string? Description { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public LivestreamStatus Status { get; set; }
        
        // Course info
        public string? CourseName { get; set; }
        
        // Calculated fields
        public DateTime EndDateTime => ScheduledDateTime.AddMinutes(DurationMinutes);
        public bool IsLive => Status == LivestreamStatus.LIVE;
        public bool IsScheduled => Status == LivestreamStatus.SCHEDULED;
        public bool CanStart => Status == LivestreamStatus.SCHEDULED && DateTime.UtcNow >= ScheduledDateTime;
    }
}
