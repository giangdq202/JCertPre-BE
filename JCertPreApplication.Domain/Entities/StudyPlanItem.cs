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
        public Guid itemId { get; set; }
        public Guid planId { get; set; }
        public int sequence { get; set; }
        public string itemType { get; set; }
        public int itemIdRef { get; set; }
        public ItemStatus status { get; set; }

        // Navigation property
        public virtual StudyPlan StudyPlan { get; set; }
    }
}
