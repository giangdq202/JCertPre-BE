using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class InstructorProfile
    {
       

        [Key]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string introduction { get; set; }

        [MaxLength(255)]
        public string experience { get; set; }

        [MaxLength(50)]
        public string teachingStyle { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
