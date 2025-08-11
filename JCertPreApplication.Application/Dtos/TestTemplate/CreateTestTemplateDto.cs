using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Application.Dtos;

public class CreateTestTemplateDto
{
    [Required(ErrorMessage = "TestTemplateTypeId is required.")]
    [NotDefaultGuid(ErrorMessage = "TestTemplateTypeId must be a valid GUID.")]
    public Guid TestTemplateTypeId { get; set; }

    [Required(ErrorMessage = "Template name is required.")]
    [MinLength(3, ErrorMessage = "Template name must be at least 3 characters.")]
    [MaxLength(200, ErrorMessage = "Template name cannot exceed 200 characters.")]
    public string templateName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration minutes is required.")]
    [Range(1, 1000, ErrorMessage = "Duration minutes must be between 1 and 1000.")]
    public int durationMinutes { get; set; }

    [Required(ErrorMessage = "Total score is required.")]
    [Range(1, 10000, ErrorMessage = "Total score must be between 1 and 10000.")]
    public int totalScore { get; set; }

    [Required(ErrorMessage = "To pass percentage is required.")]
    [Range(0, 100, ErrorMessage = "To pass percentage must be between 0 and 100.")]
    public decimal toPassPercentage { get; set; }

    [Required(ErrorMessage = "Sequence is required.")]
    [Range(1, 1000, ErrorMessage = "Sequence must be between 1 and 1000.")]
    public int sequence { get; set; }
}