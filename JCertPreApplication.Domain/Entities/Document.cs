namespace JCertPreApplication.Domain.Entities
{
    public class Document
    {
        public Guid documentId { get; set; }
        public Guid lessonId { get; set; }
        public string documentName { get; set; } = null!;
        public string fileUrl { get; set; } = null!;
        public DateTime uploadedAt { get; set; }

        // Navigation property
        public virtual Lesson Lesson { get; set; } = null!;
    }
}
