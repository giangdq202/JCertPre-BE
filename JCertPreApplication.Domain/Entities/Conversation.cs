using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Conversation
    {
        [Key]
        public Guid conversationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string conversationName { get; set; }

        [Required]
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual ICollection<ConversationParticipant> Participants { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
