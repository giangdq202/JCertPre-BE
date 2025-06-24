using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Enrollment
    {
        [Key]
        public Guid enrollmentId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public Guid courseId { get; set; }

        [Required]
        public DateTime enrollDate { get; set; }

        [Required]
        public decimal price { get; set; }

        

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
    }
}
