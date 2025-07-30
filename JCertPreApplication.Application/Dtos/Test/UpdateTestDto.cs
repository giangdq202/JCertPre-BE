using JCertPreApplication.Domain.Enums;
using System;

namespace JCertPreApplication.Application.Dtos.Test
{
    /// <summary>
    /// DTO for updating a test.
    /// - Status cannot be updated here.
    /// - TestType cannot be updated if there are questions.
    /// - DurationMinutes can only be updated if test type is CustomManual.
    /// - TestTemplateTypeId can only be updated if no questions and test type is JLPTAuto.
    /// </summary>
    public class UpdateTestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TestType? TestType { get; set; } // Cannot update if there are questions
        public int? DurationMinutes { get; set; } // Only for CustomManual
        public Guid? TestTemplateTypeId { get; set; } // Only for JLPTAuto and no questions
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int? MaxAttempts { get; set; }
        public decimal? PassingPercentage { get; set; } // <-- Add this
    }
}
