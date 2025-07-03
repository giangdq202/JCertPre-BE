using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.StudyPlan
{
    public class StudyPlanDto
    {
        public Guid PlanId { get; set; }
        public Guid StudentId { get; set; }
        public Guid CreatedByStaffId { get; set; }
        public string PlanName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
