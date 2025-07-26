using JCertPreApplication.Domain.Enums;

public class UpdateTestTemplateDto
{
    public string? templateName { get; set; }
    public int? durationMinutes { get; set; }
    public string? totalScore { get; set; }
    public decimal? toPassPercentage { get; set; }
}