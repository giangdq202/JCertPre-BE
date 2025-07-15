using JCertPreApplication.Domain.Enums;

public class TestDto
{
    public Guid TestId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public TestType TestType { get; set; }
    public int DurationMinutes { get; set; }
    public Guid? LessonId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
    public int MaxAttempts { get; set; }
    public TestStatus Status { get; set; } // <-- Added
}