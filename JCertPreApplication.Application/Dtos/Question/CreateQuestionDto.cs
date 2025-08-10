using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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
        public QuestionDifficulty Difficulty { get; set; }
        public bool IsActive { get; set; }

        // Add these for subcontent selection
        [Required]
        public ContentName ContentName { get; set; }
        [Required]
        public CourseLevel Level { get; set; }
        [Required]
        public SubContentName SubContentName { get; set; }

        public IFormFile? AudioFile { get; set; }
    }
}