using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Dtos;
namespace JCertPreApplication.Application.Dtos.TestTemplateType;
public class CreateTestTemplateTypeDto
{
    [Required(ErrorMessage = "UserId is required.")]
    [NotDefaultGuid(ErrorMessage = "UserId must be a valid GUID.")]
    public Guid userId { get; set; }

    [Required(ErrorMessage = "Type name is required.")]
    [MinLength(3, ErrorMessage = "Type name must be at least 3 characters.")]
    [MaxLength(200, ErrorMessage = "Type name cannot exceed 200 characters.")]
    public string typeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Course level is required.")]
    public CourseLevel courseLevel { get; set; }

    [Required(ErrorMessage = "Test type is required.")]
    public TestType testType { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Total test score is required.")]
    [Range(1, 1000, ErrorMessage = "Total test score must be between 1 and 1000.")]
    public int totalTestScore { get; set; }

    [Required(ErrorMessage = "Total pass percentage is required.")]
    [Range(0, 100, ErrorMessage = "Total pass percentage must be between 0 and 100.")]
    public decimal totalPassPercentage { get; set; }
}