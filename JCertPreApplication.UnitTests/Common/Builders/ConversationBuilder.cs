using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.UnitTests.Common.Builders
{
    /// <summary>
    /// Builder pattern for creating Conversation test data
    /// </summary>
    public class ConversationBuilder
    {
        private Conversation _conversation;

        public ConversationBuilder()
        {
            _conversation = new Conversation
            {
                conversationId = Guid.NewGuid(),
                conversationName = "Liên hệ tư vấn lộ trình học",
                createdAt = DateTime.UtcNow,
                Participants = new List<User>(),
                Messages = new List<Message>()
            };
        }

        public static ConversationBuilder Create() => new ConversationBuilder();

        public ConversationBuilder WithId(Guid id)
        {
            _conversation.conversationId = id;
            return this;
        }

        public ConversationBuilder WithName(string name)
        {
            _conversation.conversationName = name;
            return this;
        }

        public ConversationBuilder WithCreatedAt(DateTime createdAt)
        {
            _conversation.createdAt = createdAt;
            return this;
        }

        public ConversationBuilder WithParticipants(List<User> participants)
        {
            _conversation.Participants = participants;
            return this;
        }

        public ConversationBuilder WithMessages(List<Message> messages)
        {
            _conversation.Messages = messages;
            return this;
        }

        public ConversationBuilder AddParticipant(User user)
        {
            _conversation.Participants.Add(user);
            return this;
        }

        public ConversationBuilder AddMessage(Message message)
        {
            _conversation.Messages.Add(message);
            return this;
        }

        public Conversation Build() => _conversation;
    }
}
