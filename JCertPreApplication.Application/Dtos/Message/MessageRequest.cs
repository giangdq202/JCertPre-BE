namespace JCertPreApplication.Application.Dtos.Message
{
    public class MessageRequest
    {
        public required string Content { get; set; }
        public Guid senderId { get; set; }
    }
}
