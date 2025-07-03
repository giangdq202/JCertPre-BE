namespace JCertPreApplication.Domain.Entities
{
    public class Conversation
    {
        public Guid conversationId { get; set; }
        public string conversationName { get; set; } = null!;
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual ICollection<User> Participants { get; set; } = new List<User>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
