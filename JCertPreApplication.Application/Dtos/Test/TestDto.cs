using JCertPreApplication.Domain.Enums;

public class TestDto
{
    public Guid TestId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TestType TestType { get; set; }
    public CourseLevel CourseLevel { get; set; }
    public int DurationMinutes { get; set; }
    public Guid? LessonId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? TestTemplateTypeId { get; set; }
    public string? TestTemplateTypeName { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
    public int MaxAttempts { get; set; }
    public decimal PassingPercentage { get; set; } // <-- Added
    public TestStatus Status { get; set; }
}