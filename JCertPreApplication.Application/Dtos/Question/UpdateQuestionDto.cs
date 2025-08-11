using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class UpdateQuestionDto
    {
        [MinLength(10, ErrorMessage = "Question content must be at least 10 characters.")]
        public string? Content { get; set; }

        [MaxLength(1000, ErrorMessage = "Explanation cannot exceed 1000 characters.")]
        public string? Explanation { get; set; }

        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100.")]
        public int? Points { get; set; }

        public QuestionDifficulty? Difficulty { get; set; }
        public bool? IsActive { get; set; }

        public ContentName? ContentName { get; set; }
        public CourseLevel? Level { get; set; }
        public SubContentName? SubContentName { get; set; }

        public IFormFile? AudioFile { get; set; }
    }
}