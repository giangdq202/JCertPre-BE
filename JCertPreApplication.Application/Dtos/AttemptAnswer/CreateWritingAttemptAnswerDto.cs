
namespace JCertPreApplication.Application.Dtos.AttemptAnswer
{
    /// <summary>
    /// DTO for creating or updating a writing attempt answer.
    /// Used for writing tests where ChoiceId is always null and WrittenAnswer is required.
    /// </summary>
    public class CreateWritingAttemptAnswerDto
    {
        public Guid AttemptId { get; set; }
        public Guid QuestionId { get; set; }
        public string WrittenAnswer { get; set; } = string.Empty;
    }
}