using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.Application.Dtos.Livestream
{
    public class MuteParticipantDto
    {
        [Required]
        public bool Muted { get; set; } = true;
        
        public bool Audio { get; set; } = true;
        
        public bool Video { get; set; } = false;
    }
}
