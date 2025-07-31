using JCertPreApplication.Domain.Enums;

public class UpdateTestTemplateDto
{
    public string? templateName { get; set; }
    public int? durationMinutes { get; set; }
    public int? totalScore { get; set; }
    public decimal? toPassPercentage { get; set; }
    public int? sequence { get; set; }
}