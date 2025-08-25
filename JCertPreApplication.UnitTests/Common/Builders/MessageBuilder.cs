using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    /// <summary>
    /// Builder pattern for creating Message test data
    /// </summary>
    public class MessageBuilder
    {
        private Message _message;

        public MessageBuilder()
        {
            _message = new Message
            {
                messageId = Guid.NewGuid(),
                senderId = Guid.NewGuid(),
                conversationId = Guid.NewGuid(),
                content = "Test message content",
                sentAt = DateTime.UtcNow
            };
        }

        public static MessageBuilder Create() => new MessageBuilder();

        public MessageBuilder WithId(Guid id)
        {
            _message.messageId = id;
            return this;
        }

        public MessageBuilder WithSenderId(Guid senderId)
        {
            _message.senderId = senderId;
            return this;
        }

        public MessageBuilder WithConversationId(Guid conversationId)
        {
            _message.conversationId = conversationId;
            return this;
        }

        public MessageBuilder WithContent(string content)
        {
            _message.content = content;
            return this;
        }

        public MessageBuilder WithSentAt(DateTime sentAt)
        {
            _message.sentAt = sentAt;
            return this;
        }

        public MessageBuilder WithUser(User user)
        {
            _message.User = user;
            return this;
        }

        public MessageBuilder WithConversation(Conversation conversation)
        {
            _message.Conversation = conversation;
            return this;
        }

        public Message Build() => _message;
    }
}
