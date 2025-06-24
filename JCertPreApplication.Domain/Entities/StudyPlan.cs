using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class StudyPlan
    {
        [Key]
        public Guid planId { get; set; }

        [Required]
        [ForeignKey("Student")]
        public Guid studentId { get; set; }

        [Required]
        [ForeignKey("Staff")]
        public Guid createdByStaffId { get; set; }

        [Required]
        [MaxLength(100)]
        public string planName { get; set; }
        [Required]
        [MaxLength(1000)]
        public string description { get; set; }

        [Required]
        public DateTime startDate { get; set; }

        [Required]
        public DateTime endDate { get; set; }

        // Navigation properties
        public virtual User Student { get; set; }
        public virtual User Staff { get; set; }
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; }
    }
}
