using JCertPreApplication.Application.Dtos.User;

namespace JCertPreApplication.Application.Dtos.Conversation
{
    /// <summary>
    /// Data transfer object for conversations with participants and messages
    /// </summary>
    public class ConversationDto
    {
        public Guid ConversationId { get; set; }
        public string ConversationName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<AppUserDto> Participants { get; set; } = new();
        public List<MessageDto> Messages { get; set; } = new();
    }
} 