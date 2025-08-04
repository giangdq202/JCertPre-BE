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
        public TestType? TestType { get; set; }
        public CourseLevel? CourseLevel { get; set; } 
        public int? DurationMinutes { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int? MaxAttempts { get; set; }
        public decimal? PassingPercentage { get; set; } 
    }
}
