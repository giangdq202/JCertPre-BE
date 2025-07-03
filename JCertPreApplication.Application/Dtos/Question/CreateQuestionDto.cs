using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class CreateQuestionDto
    {
        [Required(ErrorMessage = "Question content is required")]
        [MinLength(10, ErrorMessage = "Question content must be at least 10 characters")]
        public string Content { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100")]
        public int Points { get; set; } = 1;
    }
} 