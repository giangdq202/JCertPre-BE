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
        public Guid conversationId { get; set; }
        public string conversationName { get; set; }
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual ICollection<User> Participants { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
