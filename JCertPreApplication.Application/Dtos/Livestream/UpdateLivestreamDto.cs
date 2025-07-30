using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Livestream
{
    public class UpdateLivestreamDto
    {
        [StringLength(500)]
        public string? Description { get; set; }
        
        public DateTime? ScheduledDateTime { get; set; }
        
        [Range(15, 480, ErrorMessage = "Duration must be between 15 and 480 minutes")]
        public int? DurationMinutes { get; set; }
    }
}
