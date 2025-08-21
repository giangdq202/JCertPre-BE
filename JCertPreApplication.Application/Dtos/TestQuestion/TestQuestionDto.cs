namespace JCertPreApplication.Application.Dtos.TestQuestion;
public class TestQuestionDto
{
    public Guid TestQuestionId { get; set; }
    public Guid TestId { get; set; }
    public Guid QuestionId { get; set; }
    public int QuestionNumber { get; set; }
    public int? PartNumber { get; set; }
    public int? PartDurationMinutes { get; set; }
}