using JCertPreApplication.Domain.Enums;
namespace JCertPreApplication.Application.Dtos.TestTemplate;
public class TestTemplateDto
{
    public Guid templateId { get; set; }
    public Guid TestTemplateTypeId { get; set; }
    public string templateName { get; set; } = null!;
    public int durationMinutes { get; set; }
    public int totalScore { get; set; }
    public decimal toPassPercentage { get; set; }
    public int sequence { get; set; }
}