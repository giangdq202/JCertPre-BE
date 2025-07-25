namespace JCertPreApplication.Domain.Entities
{
    public class Livestream
    {
        public Guid livestreamId { get; set; }
        public Guid lessonId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }

        // 1-1 relation: Each livestream belongs to one lesson
        public virtual Lesson Lesson { get; set; } = null!;
    }
}
