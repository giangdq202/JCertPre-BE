using JCertPreApplication.Domain.Enums;
using System;

namespace JCertPreApplication.Application.Dtos.Test
{
    /// <summary>
    /// DTO for creating a test.
    /// - Status is set by the system.
    /// - DurationMinutes is only used for CustomManual.
    /// - TestTemplateTypeId is required for JLPTAuto.
    /// - PassingPercentage is required for test score summary.
    /// </summary>
    public class CreateTestDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TestType TestType { get; set; }
        public CourseLevel CourseLevel { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int MaxAttempts { get; set; }
        public decimal PassingPercentage { get; set; } 
    }
}
