using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.TestTemplateTypes;
public class UpdateTestTemplateTypeDto
{
    [MinLength(3, ErrorMessage = "Type name must be at least 3 characters.")]
    [MaxLength(200, ErrorMessage = "Type name cannot exceed 200 characters.")]
    public string? typeName { get; set; }

    public CourseLevel? courseLevel { get; set; }
    public TestType? testType { get; set; }

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? description { get; set; }

    public bool? isActive { get; set; }

    [Range(1, 1000, ErrorMessage = "Total test score must be between 1 and 1000.")]
    public int? totalTestScore { get; set; }

    [Range(0, 100, ErrorMessage = "Total pass percentage must be between 0 and 100.")]
    public decimal? totalPassPercentage { get; set; }
}