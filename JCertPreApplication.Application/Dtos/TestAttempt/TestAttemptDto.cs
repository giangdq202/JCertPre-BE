using JCertPreApplication.Domain.Enums;

public class TestAttemptDto
{
    public Guid AttemptId { get; set; }
    public Guid UserId { get; set; }
    public Guid TestId { get; set; }
    public int AttemptNumber { get; set; }
    public TestAttemptStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int? TotalScore { get; set; }
    public int? LanguageKnowledgeScore { get; set; }
    public int? ReadingScore { get; set; }
    public int? ListeningScore { get; set; }
    public bool? IsPass { get; set; }
}