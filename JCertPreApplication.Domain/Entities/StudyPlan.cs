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
        public Guid planId { get; set; }
        public Guid studentId { get; set; }
        public Guid createdByStaffId { get; set; }
        public string planName { get; set; }
        public string description { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        // Navigation properties
        public virtual User Student { get; set; }
        public virtual User Staff { get; set; }
        public virtual ICollection<StudyPlanItem> StudyPlanItems { get; set; }
    }
}
