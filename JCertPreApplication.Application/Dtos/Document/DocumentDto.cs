namespace JCertPreApplication.Application.Dtos.Document
{
    public class DocumentDto
    {
        public Guid documentId { get; set; }
        public Guid lessonId { get; set; }
        public string documentName { get; set; } = null!;
        public string fileUrl { get; set; } = null!;
        public DateTime uploadedAt { get; set; }
    }
} 