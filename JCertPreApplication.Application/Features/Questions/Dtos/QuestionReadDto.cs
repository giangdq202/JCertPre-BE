using System;

namespace JCertPreApplication.Application.Features.Questions.Dtos
{
    public class QuestionReadDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string? Explanation { get; set; }
        public Guid TagId { get; set; }
    }
}