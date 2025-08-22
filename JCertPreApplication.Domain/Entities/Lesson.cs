namespace JCertPreApplication.Domain.Entities
{
    public class Lesson
    {
        public Guid lessonId { get; set; }
        public Guid courseId { get; set; }
        public string title { get; set; } = null!;
        public int lessonOrder { get; set; }
        public string content { get; set; } = null!;
        public string? comment { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual Test? Test { get; set; } // 1-1 with Test
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    }
}
