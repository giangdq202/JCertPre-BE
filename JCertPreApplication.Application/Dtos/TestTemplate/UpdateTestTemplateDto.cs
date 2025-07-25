using JCertPreApplication.Domain.Enums;

public class UpdateTestTemplateDto
{
    public string TemplateName { get; set; } = null!;
    public CourseLevel CourseLevel { get; set; }
    public TestType TestType { get; set; }
    public string DurationMinutes { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ThreeFirstParts { get; set; }
    public string? FourFirstParts { get; set; }
    public string? Reading { get; set; }
    public string Listening { get; set; } = null!;
    public string Total { get; set; } = null!;
    public bool IsActive { get; set; }
}