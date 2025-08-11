using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class CreateQuestionDto
    {
        [Required(ErrorMessage = "Question content is required.")]
        [MinLength(10, ErrorMessage = "Question content must be at least 10 characters.")]
        [MaxLength(1000, ErrorMessage = "Question content cannot exceed 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Explanation cannot exceed 1000 characters.")]
        public string? Explanation { get; set; }

        [Required(ErrorMessage = "Points are required.")]
        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100.")]
        public int Points { get; set; } = 1;

        [Required(ErrorMessage = "Difficulty is required.")]
        public QuestionDifficulty Difficulty { get; set; }

        [Required(ErrorMessage = "IsActive is required.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "ContentName is required.")]
        public ContentName ContentName { get; set; }

        [Required(ErrorMessage = "Level is required.")]
        public CourseLevel Level { get; set; }

        [Required(ErrorMessage = "SubContentName is required.")]
        public SubContentName SubContentName { get; set; }

        public IFormFile? AudioFile { get; set; }
    }
}