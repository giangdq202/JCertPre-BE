using JCertPreApplication.Domain.Enums;
namespace JCertPreApplication.Application.Dtos.TestAttempt;
public class TestAttemptDto
{
    public Guid AttemptId { get; set; }
    public Guid UserId { get; set; }
    public Guid TestId { get; set; }
    public int AttemptNumber { get; set; }
    public TestAttemptStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool? IsPass { get; set; }
}