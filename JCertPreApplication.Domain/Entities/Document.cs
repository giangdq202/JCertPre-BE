using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Document
    {
        [Key]
        public Guid documentId { get; set; }

        [Required]
        [ForeignKey("Lesson")]
        public Guid lessonId { get; set; }

        [Required]
        [MaxLength(100)]
        public string documentName { get; set; }

        [Required]
        public string fileUrl { get; set; }

        [Required]
        public DateTime uploadedAt { get; set; }

        // Navigation property
        public virtual Lesson Lesson { get; set; }
    }
}
