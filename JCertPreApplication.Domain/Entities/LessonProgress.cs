

namespace JCertPreApplication.Domain.Entities
{
    public class LessonProgress
    {
        public Guid progressId { get; set; }
        public Guid userId { get; set; }
        public Guid lessonId { get; set; }
        public decimal completionRate { get; set; } = 0.0m;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;
    }
}