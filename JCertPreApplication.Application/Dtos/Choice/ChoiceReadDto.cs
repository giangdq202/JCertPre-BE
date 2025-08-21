namespace JCertPreApplication.Application.Dtos.Choice;
public class ChoiceReadDto
{
    public Guid ChoiceId { get; set; }
    public Guid QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}