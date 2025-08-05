using JCertPreApplication.Domain.Enums;

public class TestTemplateTypeDto
{
    public Guid TestTemplateTypeId { get; set; }
    public Guid userId { get; set; }
    public string typeName { get; set; } = null!;
    public CourseLevel courseLevel { get; set; }
    public TestType testType { get; set; }
    public string description { get; set; } = null!;
    public bool isActive { get; set; }
    public DateTime createdAt { get; set; }
    public int totalTestScore { get; set; }
    public decimal totalPassPercentage { get; set; }
}