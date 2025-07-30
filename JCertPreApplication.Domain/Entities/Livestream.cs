using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Livestream
    {
        public Guid livestreamId { get; set; }
        public Guid courseId { get; set; }
        public string? description { get; set; }
        public DateTime scheduledDateTime { get; set; }
        public int durationMinutes { get; set; }
        public LivestreamStatus status { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; } = null!;
    }
}
