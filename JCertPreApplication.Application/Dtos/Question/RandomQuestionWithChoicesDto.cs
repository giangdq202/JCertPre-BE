namespace JCertPreApplication.Application.Dtos.Question;
public class RandomQuestionWithChoicesDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public List<RandomChoiceDto> Choices { get; set; } = new();
}

public class RandomChoiceDto
{
    public Guid ChoiceId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}