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
        public Guid lessonId { get; set; }
        public Guid courseId { get; set; }
        public string title { get; set; }
        public int lessonOrder { get; set; }
        public string content { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
