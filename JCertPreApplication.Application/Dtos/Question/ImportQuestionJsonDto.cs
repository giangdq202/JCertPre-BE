using System.Collections.Generic;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class ImportQuestionJsonDto
    {
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public int Points { get; set; }
        public QuestionDifficulty Difficulty { get; set; }
        public bool IsActive { get; set; }
        public ContentName ContentName { get; set; }
        public CourseLevel Level { get; set; }
        public SubContentName SubContentName { get; set; }
        public List<ImportChoiceJsonDto> Choices { get; set; } = new();
    }

    public class ImportChoiceJsonDto
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}