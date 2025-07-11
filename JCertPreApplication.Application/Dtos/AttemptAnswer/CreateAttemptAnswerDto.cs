namespace JCertPreApplication.Application.Dtos.AttemptAnswer;
public class CreateAttemptAnswerDto
{
    public Guid AttemptId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid ChoiceId { get; set; }
}