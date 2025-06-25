namespace JCertPreApplication.Domain.Entities
{
    public class Lesson
    {
        public Guid lessonId { get; set; }
        public Guid courseId { get; set; }
        public string title { get; set; }
        public int lessonOrder { get; set; }
        public string content { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
