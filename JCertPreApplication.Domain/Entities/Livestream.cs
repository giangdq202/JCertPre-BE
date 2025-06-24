using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Livestream
    {
        [Key]
        public Guid livestreamId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public Guid courseId { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public DateTime startTime { get; set; }

        [Required]
        public DateTime endTime { get; set; }

        [Required]
        public string meetingUrl { get; set; }

        [Required]
        public string recordingUrl { get; set; }

        // Navigation property
        public virtual Course Course { get; set; }
    }
}
