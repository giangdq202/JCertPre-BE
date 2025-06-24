using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Feedback
    {
        [Key]
        public Guid feedbackId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public Guid courseId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        public int rating { get; set; }

        [Required]
        public string comment { get; set; }
        [Required]
        public string reply { get; set; }

        [Required]
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual User User { get; set; }
    }
}
