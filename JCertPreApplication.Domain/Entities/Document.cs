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
        public Guid documentId { get; set; }
        public Guid lessonId { get; set; }
        public string documentName { get; set; }
        public string fileUrl { get; set; }
        public DateTime uploadedAt { get; set; }

        // Navigation property
        public virtual Lesson Lesson { get; set; }
    }
}
