using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Enrollment
{
    public class SelfEnrollmentRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }
    }
} 