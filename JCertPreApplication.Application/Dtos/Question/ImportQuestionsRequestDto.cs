using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class ImportQuestionsRequestDto
    {
        public IFormFile File { get; set; } = null!;
    }
}