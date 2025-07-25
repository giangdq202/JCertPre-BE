using System;

namespace JCertPreApplication.Domain.Entities
{
    public class TestTemplateConfig
    {
        public Guid configId { get; set; }
        public Guid templateId { get; set; }
        public Guid subContentId { get; set; }
        public int questionCount { get; set; }
        public int pointPerQuestion { get; set; }  
        public int totalPoints { get; set; } 
        public int sequence { get; set; }

        // Navigation properties
        public virtual TestTemplate TestTemplate { get; set; } = null!;
        public virtual SubContent SubContent { get; set; } = null!;
    }
}