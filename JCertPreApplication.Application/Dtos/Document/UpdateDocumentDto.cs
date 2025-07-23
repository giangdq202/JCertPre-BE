using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.Document
{
    public class UpdateDocumentDto
    {
        public string? documentName { get; set; }
        public IFormFile? newFile { get; set; }
    }
} 