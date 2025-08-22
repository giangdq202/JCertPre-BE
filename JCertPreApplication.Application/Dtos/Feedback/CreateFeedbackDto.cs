
using JCertPreApplication.Application.Dtos.Utilities;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Feedback
{
    public class CreateFeedbackDto
    {
        [Required(ErrorMessage = "CourseId is required.")]
        [NotDefaultGuid(ErrorMessage = "CourseId must be a valid GUID.")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        [NotDefaultGuid(ErrorMessage = "UserId must be a valid GUID.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public decimal Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string? Comment { get; set; }
    }
}