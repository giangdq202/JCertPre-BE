using System.ComponentModel.DataAnnotations;

public class UpdateTestTemplateDto
{
    [MinLength(3, ErrorMessage = "Template name must be at least 3 characters.")]
    [MaxLength(200, ErrorMessage = "Template name cannot exceed 200 characters.")]
    public string? templateName { get; set; }

    [Range(1, 1000, ErrorMessage = "Duration minutes must be between 1 and 1000.")]
    public int? durationMinutes { get; set; }

    [Range(1, 10000, ErrorMessage = "Total score must be between 1 and 10000.")]
    public int? totalScore { get; set; }

    [Range(0, 100, ErrorMessage = "To pass percentage must be between 0 and 100.")]
    public decimal? toPassPercentage { get; set; }

    [Range(1, 1000, ErrorMessage = "Sequence must be between 1 and 1000.")]
    public int? sequence { get; set; }
}