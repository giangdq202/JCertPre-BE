namespace JCertPreApplication.Application.Dtos.Choice
{
    public class ChoiceCreateDto
    {
        public Guid QuestionId { get; set; }
        public required string Content { get; set; }
        public bool IsCorrect { get; set; }
        public string? Explanation { get; set; }
    }
} 