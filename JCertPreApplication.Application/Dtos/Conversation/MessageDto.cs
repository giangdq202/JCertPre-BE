namespace JCertPreApplication.Application.Dtos.Conversation
{
    /// <summary>
    /// Data transfer object for messages in conversations
    /// </summary>
    public class MessageDto
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public Guid ConversationId { get; set; }
        public DateTime SentAt { get; set; }
    }
} 