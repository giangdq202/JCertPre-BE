using JCertPreApplication.Domain.Enums;

public class UpdateTestTemplateTypeDto
{
    public string? typeName { get; set; }
    public CourseLevel? courseLevel { get; set; }
    public TestType? testType { get; set; }
    public string? description { get; set; }
    public bool? isActive { get; set; }
    public int? totalTestScore { get; set; }
    public decimal? totalPassPercentage { get; set; }
}