using JCertPreApplication.Domain.Enums;

public class TestTemplateTypeDto
{
    public Guid TestTemplateTypeId { get; set; }
    public Guid userId { get; set; }
    public string? CreatedByUserName { get; set; } // NEW
    public Guid? verifiedUserId { get; set; }
    public string? VerifiedByUserName { get; set; } // NEW
    public string typeName { get; set; }
    public CourseLevel courseLevel { get; set; }
    public TestType testType { get; set; }
    public string description { get; set; }
    public bool isActive { get; set; }
    public DateTime createdAt { get; set; }
    public int totalTestScore { get; set; }
    public decimal totalPassPercentage { get; set; }
}