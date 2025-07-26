using JCertPreApplication.Domain.Enums;

public class CreateTestTemplateDto
{
    public Guid TestTemplateTypeId { get; set; }
    public string templateName { get; set; } = null!;
    public int durationMinutes { get; set; }
    public string totalScore { get; set; } = null!;
    public decimal toPassPercentage { get; set; }
}