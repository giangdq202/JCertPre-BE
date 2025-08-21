using JCertPreApplication.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public TestType? TestType { get; set; }
        public CourseLevel? CourseLevel { get; set; }

        [Range(1, 1000, ErrorMessage = "DurationMinutes must be between 1 and 1000.")]
        public int? DurationMinutes { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        [Range(1, 100, ErrorMessage = "MaxAttempts must be between 1 and 100.")]
        public int? MaxAttempts { get; set; }

        [Range(0, 100, ErrorMessage = "PassingPercentage must be between 0 and 100.")]
        public decimal? PassingPercentage { get; set; }
    }
}
