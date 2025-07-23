using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.Document
{
    public class CreateDocumentDto
    {
        public Guid lessonId { get; set; }
        public IFormFile file { get; set; } = null!;
    }
} 