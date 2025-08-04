using JCertPreApplication.Domain.Enums;

public class CreateTestTemplateTypeDto
{
    public Guid userId { get; set; }
    public string typeName { get; set; } = null!;
    public CourseLevel courseLevel { get; set; }
    public TestType testType { get; set; }
    public string description { get; set; } = null!;
}