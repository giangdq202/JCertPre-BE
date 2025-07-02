using JCertPreApplication.Application.Dtos.Conversation;
using JCertPreApplication.Application.Dtos.Message;

namespace JCertPreApplication.Application.Features.Conversation
{
    /// <summary>
    /// Service interface for conversation management operations
    /// </summary>
    public interface IConversationService
    {
        /// <summary>
        /// Creates a new conversation for a student with an academic manager
        /// </summary>
        /// <param name="studentId">The ID of the student initiating the conversation</param>
        /// <returns>The created conversation details</returns>
        /// <exception cref="ApiException">Thrown when student is invalid or no academic manager available</exception>
        Task<ConversationDto> CreateConversationAsync(Guid studentId);

        /// <summary>
        /// Sends a message in a conversation
        /// </summary>
        /// <param name="conversationId">The conversation ID</param>
        /// <param name="senderId">The authenticated sender ID from token</param>
        /// <param name="messageRequest">The message content and details</param>
        /// <returns>The sent message details</returns>
        /// <exception cref="ApiException">Thrown when conversation not found, sender not found, or message content is invalid</exception>
        Task<MessageDto> SendMessageAsync(Guid conversationId, Guid senderId, MessageRequest messageRequest);

        /// <summary>
        /// Assigns an instructor to a conversation
        /// </summary>
        /// <param name="conversationId">The conversation ID</param>
        /// <param name="instructorId">The instructor ID to assign</param>
        /// <exception cref="ApiException">Thrown when conversation or instructor not found, or user is not an instructor</exception>
        Task AssignInstructorAsync(Guid conversationId, Guid instructorId);

        /// <summary>
        /// Gets conversation details with messages and participants
        /// </summary>
        /// <param name="conversationId">The conversation ID</param>
        /// <returns>The conversation details</returns>
        /// <exception cref="ApiException">Thrown when conversation not found</exception>
        Task<ConversationDto> GetConversationAsync(Guid conversationId);

        /// <summary>
        /// Gets all conversations for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of conversations the user participates in</returns>
        Task<IEnumerable<ConversationDto>> GetConversationsForUserAsync(Guid userId);
    }
}
