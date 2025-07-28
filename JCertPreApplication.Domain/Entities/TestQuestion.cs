using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class TestQuestion
    {
        public Guid testQuestionId { get; set; }
        public Guid testId { get; set; }
        public Guid questionId { get; set; }
        public int questionNumber { get; set; } = 0;
        public int? partNumber { get; set; }
        public int? partDurationMinutes { get; set; }
        public bool isActive { get; set; }


        // Navigation properties
        public virtual Test Test { get; set; } = null!;
        public virtual Question Question { get; set; } = null!;
    }
}
