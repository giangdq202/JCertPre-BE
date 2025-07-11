using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Enrollment
{
    public class EnrollmentRequestDto
    {
        [Required]
        public Guid CourseId { get; set; }
    }
} 