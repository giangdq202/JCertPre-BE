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
        [Key]
        public Guid choiceId { get; set; }

        [Required]
        [ForeignKey("Question")]
        public Guid questionId { get; set; }

        [Required]
        public string choiceText { get; set; }

        [Required]
        public bool isCorrect { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; }
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; }
    }
}
