
namespace JCertPreApplication.Application.Dtos.AttemptAnswer
{
    /// <summary>
    /// DTO for returning details of a written attempt answer.
    /// </summary>
    public class WrittenAttemptAnswerDetailDto
    {
        public Guid AnswerId { get; set; }
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; }
        public string WrittenAnswer { get; set; } = string.Empty;
        public string? GraderComment { get; set; }
        public int Score { get; set; }
    }
}