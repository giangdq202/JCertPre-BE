namespace JCertPreApplication.Domain.Entities
{
    public class Lesson
    {
        public Guid lessonId { get; set; }
        public Guid courseId { get; set; }
        public string title { get; set; } = null!;
        public int lessonOrder { get; set; }
        public string content { get; set; } = null!;
        public string? comment { get; set; } // Nullable comment field

        // Navigation properties
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
        public virtual ICollection<LessonProgress> LessonProgresses { get; set; } = new List<LessonProgress>();
    }
}
