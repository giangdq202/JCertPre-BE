namespace JCertPreApplication.Domain.Entities
{
    public class Message
    {
        public Guid messageId { get; set; }
        public Guid senderId { get; set; }
        public Guid conversationId { get; set; }
        public string content { get; set; }
        public DateTime sentAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Conversation Conversation { get; set; }
    }
}
