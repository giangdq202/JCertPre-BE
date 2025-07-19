using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class UpdateQuestionDto
    {
        public string? Content { get; set; }
        public string? Explanation { get; set; }
        public int? Points { get; set; }
        public QuestionDifficulty? Difficulty { get; set; }

        // Optional: allow updating subcontent
        public ContentName? ContentName { get; set; }
        public CourseLevel? Level { get; set; }
        public SubContentName? SubContentName { get; set; }
    }
}