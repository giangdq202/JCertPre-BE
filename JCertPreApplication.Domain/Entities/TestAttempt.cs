using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JCertPreApplication.Domain.Entities
{
    public class TestAttempt
    {
        [Key]
        public Guid attemptId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        [ForeignKey("Test")]
        public Guid testId { get; set; }

        [Required]
        public DateTime startTime { get; set; }

        [Required]
        public DateTime endTime { get; set; }

        [Required]
        public int totalScore { get; set; }

        [Required]
        public int languageKnowledgeScore { get; set; }

        [Required]
        public int readingScore { get; set; }

        [Required]
        public int listeningScore { get; set; }

        [Required]
        public bool isPass { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; }
    }
}
