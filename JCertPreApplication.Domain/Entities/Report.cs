using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Report
    {
        [Key]
        public Guid reportId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid reporterStudentId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid reportedInstructorId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string reportContent { get; set; }

        [Required]
        public string status { get; set; }

        [Required]
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual User StudentUser { get; set; }
        public virtual User InstructorUser { get; set; }
    }
}
