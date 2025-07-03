using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class UpdateQuestionDto
    {
        [MinLength(10, ErrorMessage = "Question content must be at least 10 characters")]
        public string? Content { get; set; }

        public string? Explanation { get; set; }

        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100")]
        public int? Points { get; set; }
    }
} 