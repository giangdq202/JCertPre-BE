using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class StudyPlanItem
    {
        [Key]
        public Guid itemId { get; set; }

        [Required]
        [ForeignKey("StudyPlan")]
        public Guid planId { get; set; }

        [Required]
        public int sequence { get; set; }

        [Required]
        [MaxLength(50)]
        public string itemType { get; set; }

        [Required]
        public int itemIdRef { get; set; }

        [Required]
        public ItemStatus status { get; set; }

        // Navigation property
        public virtual StudyPlan StudyPlan { get; set; }
    }
}
