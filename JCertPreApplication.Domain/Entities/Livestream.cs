namespace JCertPreApplication.Domain.Entities
{
    public class Livestream
    {
        public Guid livestreamId { get; set; }
        public Guid courseId { get; set; }
        public string title { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string meetingUrl { get; set; }
        public string? recordingUrl { get; set; }

        // Navigation property
        public virtual Course Course { get; set; }
    }
}
