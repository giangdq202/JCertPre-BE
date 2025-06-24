using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class ConversationParticipant
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Conversation")]
        public Guid conversationId { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        public DateTime joinedAt { get; set; }

        // Navigation properties
        public virtual Conversation Conversation { get; set; }
        public virtual User User { get; set; }
    }
}
