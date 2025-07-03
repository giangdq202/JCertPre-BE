using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Dtos.StudyPlan
{
    public class StudyPlanItemDto
    {
        public Guid ItemId { get; set; }
        public Guid PlanId { get; set; }
        public int Sequence { get; set; }
        public string ItemType { get; set; } = null!;
        public Guid? CourseId { get; set; }
        public Guid? TestId { get; set; }
        public ItemStatus Status { get; set; }
    }
}
