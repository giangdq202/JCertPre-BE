namespace JCertPreApplication.Application.Dtos.Choice
{
    public class ChoiceReadDto
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public required string Content { get; set; }
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
    }
} 