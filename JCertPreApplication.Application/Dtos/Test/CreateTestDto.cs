using System.ComponentModel.DataAnnotations;
using JCertPreApplication.Domain.Enums;

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
        [Required(ErrorMessage = "Title is required.")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "TestType is required.")]
        public TestType TestType { get; set; }

        [Required(ErrorMessage = "CourseLevel is required.")]
        public CourseLevel CourseLevel { get; set; }

        [Required(ErrorMessage = "DurationMinutes is required.")]
        [Range(1, 1000, ErrorMessage = "DurationMinutes must be between 1 and 1000.")]
        public int DurationMinutes { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        [Required(ErrorMessage = "MaxAttempts is required.")]
        [Range(1, 100, ErrorMessage = "MaxAttempts must be between 1 and 100.")]
        public int MaxAttempts { get; set; }

        [Required(ErrorMessage = "PassingPercentage is required.")]
        [Range(0, 100, ErrorMessage = "PassingPercentage must be between 0 and 100.")]
        public decimal PassingPercentage { get; set; } 
    }
}
