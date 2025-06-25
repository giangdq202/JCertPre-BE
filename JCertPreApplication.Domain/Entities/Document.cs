namespace JCertPreApplication.Domain.Entities
{
    public class Document
    {
        public Guid documentId { get; set; }
        public Guid lessonId { get; set; }
        public string documentName { get; set; }
        public string fileUrl { get; set; }
        public DateTime uploadedAt { get; set; }

        // Navigation property
        public virtual Lesson Lesson { get; set; }
    }
}
