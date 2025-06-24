using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class QuestionAttachment
    {
        [Key]
        public Guid attachmentId { get; set; }

        [Required]
        [ForeignKey("Question")]
        public Guid questionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string mediaUrl { get; set; }

        [Required]
        public MediaType mediaType { get; set; }

        // Navigation property
        public virtual Question Question { get; set; }
    }
}
