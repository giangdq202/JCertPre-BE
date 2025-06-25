using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Choice
    {
        public Guid choiceId { get; set; }
        public Guid questionId { get; set; }
        public string choiceText { get; set; }
        public bool isCorrect { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; }
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; }
    }
}
