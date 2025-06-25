using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Enrollment
    {
        public Guid enrollmentId { get; set; }
        public Guid userId { get; set; }
        public Guid courseId { get; set; }
        public DateTime enrollDate { get; set; }
        public decimal price { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
    }
}
