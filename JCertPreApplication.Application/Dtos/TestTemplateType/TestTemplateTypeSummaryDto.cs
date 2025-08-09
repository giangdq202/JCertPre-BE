using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;

namespace JCertPreApplication.Application.Dtos.TestTemplateType
{
    public class TestTemplateTypeSummaryDto
    {
        public Guid TestTemplateTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public CourseLevel CourseLevel { get; set; }
        public TestType TestType { get; set; }
        public int TotalTestScore { get; set; }
        public decimal TotalPassPercentage { get; set; }
        public int TotalDurationMinutes { get; set; }
        public List<TestTemplateSummaryDto> TestTemplates { get; set; } = new();
    }
}
