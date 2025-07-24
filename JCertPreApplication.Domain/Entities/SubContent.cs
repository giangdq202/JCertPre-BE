using System;
using System.Collections.Generic;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class SubContent
    {
        public Guid SubContentId { get; set; }
        public SubContentName SubContentName { get; set; }
        public CourseLevel Level { get; set; }
        public ContentName ContentName { get; set; }

        // Navigation property
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

        public virtual ICollection<TestTemplateConfig> TestTemplateConfigs { get; set; } = new List<TestTemplateConfig>();
    }
}
