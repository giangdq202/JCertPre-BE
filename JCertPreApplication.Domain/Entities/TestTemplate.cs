using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class TestTemplate
    {
        public Guid templateId { get; set; }
        public Guid TestTemplateTypeId { get; set; }

        public string templateName { get; set; } = null!;
        public int durationMinutes { get; set; } = 0;
        public int totalScore { get; set; } = 0; 
        public decimal toPassPercentage { get; set; } = 0.0m;

        public virtual TestTemplateType TestTemplateType { get; set; } = null!;
        public virtual ICollection<TestTemplateConfig> TestTemplateConfigs { get; set; } = new List<TestTemplateConfig>();
    }
}