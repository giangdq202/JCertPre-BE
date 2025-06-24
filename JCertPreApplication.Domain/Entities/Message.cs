using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Message
    {
        [Key]
        public Guid messageId { get; set; }

        [Required]
        [ForeignKey("User")]
        public Guid senderId { get; set; }

        [Required]
        [ForeignKey("Conversation")]
        public Guid conversationId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string content { get; set; }

        [Required]
        public DateTime sentAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Conversation Conversation { get; set; }
    }
}
