
namespace JCertPreApplication.Application.Dtos.AttemptAnswer
{
    /// <summary>
    /// DTO for scoring a writing answer.
    /// </summary>
    public class ScoringWritingRequestDto
    {
        public int Score { get; set; }
        public string GraderComment { get; set; } = string.Empty;
    }
}