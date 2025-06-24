using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Lesson
    {
        [Key]
        public Guid lessonId { get; set; }

        [Required]
        [ForeignKey("Course")]
        public Guid courseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        [Required]
        public int lessonOrder { get; set; }

        [Required]
        public string content { get; set; }

        // Navigation property
        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
