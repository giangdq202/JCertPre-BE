using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Livestream
{
    public class CreateLivestreamDto
    {
        [Required]
        public Guid CourseId { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public DateTime ScheduledDateTime { get; set; }
        
        [Required]
        [Range(15, 480, ErrorMessage = "Duration must be between 15 and 480 minutes")]
        public int DurationMinutes { get; set; } = 60;
    }
}
