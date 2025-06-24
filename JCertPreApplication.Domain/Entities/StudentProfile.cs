using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class StudentProfile
    {
        [Key]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        [MaxLength(50)]
        public string currentLevel { get; set; }

        [Required]
        public string learningGoals { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
