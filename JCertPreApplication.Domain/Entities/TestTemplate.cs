using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class TestTemplate
    {
        public Guid templateId { get; set; }
        public Guid userId { get; set; }
        public string templateName { get; set; } = null!;
        public CourseLevel courseLevel { get; set; }
        public TestType testType { get; set; }
        public string durationMinutes { get; set; } = null!;
        public string description { get; set; } = null!;
        public string? threeFirstParts { get; set; }
        public string? fourFirstParts { get; set; }
        public string? reading { get; set; }
        public string listening { get; set; } = null!;
        public string total { get; set; } = null!;
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }

        // Navigation property
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<TestTemplateConfig> TestTemplateConfigs { get; set; } = new List<TestTemplateConfig>();
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    }
}