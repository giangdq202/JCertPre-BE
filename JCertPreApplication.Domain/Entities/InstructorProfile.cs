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
        public Guid userId { get; set; }
        public string introduction { get; set; }
        public string experience { get; set; }
        public string teachingStyle { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
