using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class AttemptAnswer
    {
        //[Key]
        //public Guid answerId { get; set; }
        //[Required]
        //[ForeignKey("TestAttempt")]
        //public Guid attemptId { get; set; }
        //[Required]
        //[ForeignKey("Question")]
        //public Guid questionId { get; set; }
        //[Required]
        //[ForeignKey("Choice")]
        //public Guid choiceId { get; set; }
        //// Navigation properties
        //public virtual TestAttempt TestAttempt { get; set; }
        //public virtual Question Question { get; set; }
        //public virtual Choice Choice { get; set; }
        public Guid answerId { get; set; }
        public Guid attemptId { get; set; }
        public Guid questionId { get; set; }
        public Guid choiceId { get; set; }

        // Navigation properties
        public virtual TestAttempt TestAttempt { get; set; }
        public virtual Question Question { get; set; }
        public virtual Choice Choice { get; set; }
    }
}
