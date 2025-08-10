using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class UpdateQuestionDto
    {
        public string? Content { get; set; }
        public string? Explanation { get; set; }
        public int? Points { get; set; }
        public QuestionDifficulty? Difficulty { get; set; }
        public bool? IsActive { get; set; }
        public ContentName? ContentName { get; set; }
        public CourseLevel? Level { get; set; }
        public SubContentName? SubContentName { get; set; }

        // For audio upload
        public IFormFile? AudioFile { get; set; }
    }
}