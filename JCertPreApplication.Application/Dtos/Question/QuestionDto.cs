using JCertPreApplication.Application.Dtos.Choice;

namespace JCertPreApplication.Application.Dtos.Question
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public int Points { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ChoiceReadDto>? Choices { get; set; }
    }
} 