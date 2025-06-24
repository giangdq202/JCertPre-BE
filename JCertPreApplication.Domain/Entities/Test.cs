using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Test
    {
        [Key]
        public Guid testId { get; set; }

        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        [Required]
        public string description { get; set; }

        [Required]
        [MaxLength(50)]
        public string testType { get; set; }

        [Required]
        public int durationMinutes { get; set; }



        [Required]
        [ForeignKey("Lesson")]
        public Guid lessonId { get; set; }

        [Required]
        [ForeignKey("CreatedByUser")]
        public Guid createdByUserId { get; set; }

        


        public virtual Lesson Lesson { get; set; }
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<TestAttempt> TestAttempts { get; set; }
    }
}
