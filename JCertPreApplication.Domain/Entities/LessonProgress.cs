using System;

namespace JCertPreApplication.Domain.Entities
{
    public class LessonProgress
    {
        public Guid progressId { get; set; }
        public Guid userId { get; set; }
        public Guid lessonId { get; set; }
        public bool isCompleted { get; set; } = false;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;
    }
}