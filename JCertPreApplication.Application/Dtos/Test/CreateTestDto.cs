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
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TestType TestType { get; set; }
        public int DurationMinutes { get; set; } // Only used for CustomManual, ignored otherwise
        public Guid? TestTemplateTypeId { get; set; } // Required for JLPTAuto, null otherwise
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int MaxAttempts { get; set; }
        public decimal PassingPercentage { get; set; } // <-- Added
    }
}
