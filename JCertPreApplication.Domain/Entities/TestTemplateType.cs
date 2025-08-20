using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;

namespace JCertPreApplication.Domain.Entities
{
    public class TestTemplateType
    {
        public Guid TestTemplateTypeId { get; set; }
        public Guid userId { get; set; }
        public Guid? verifiedUserId { get; set; } // NEW FIELD

        public string typeName { get; set; } = null!;
        public CourseLevel courseLevel { get; set; }
        public TestType testType { get; set; }
        public int totalTestScore { get; set; }
        public decimal totalPassPercentage { get; set; } = 0.0m;
        public string description { get; set; } = null!;
        public bool isActive { get; set; }
        public DateTime createdAt { get; set; }

        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? VerifiedByUser { get; set; } // NEW NAVIGATION
        public virtual ICollection<TestTemplate> TestTemplates { get; set; } = new List<TestTemplate>();
        public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    }
}