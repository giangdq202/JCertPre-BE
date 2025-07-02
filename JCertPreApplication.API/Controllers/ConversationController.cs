using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Conversation;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Features.Conversation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Conversation Management API Controller
    /// </summary>
    /// <remarks>
    /// Provides conversation management functionality including:
    /// - Creating conversations between students and academic managers
    /// - Sending messages within conversations
    /// - Assigning instructors to conversations
    /// - Retrieving conversation details and user conversations
    /// 
    /// Security Features:
    /// - User authentication required for most operations
    /// - Sender identity verified from JWT token
    /// - Participant validation for message sending
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Conversations")]
    [Produces("application/json")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
        }

        /// <summary>
        /// Create a new conversation for a student
        /// </summary>
        /// <remarks>
        /// Creates a new conversation between a student and a randomly assigned academic manager.
        /// 
        /// Process:
        /// 1. Validates that the provided user ID belongs to a student
        /// 2. Randomly selects an available academic manager
        /// 3. Creates a conversation with both participants
        /// 4. Returns the conversation details
        /// 
        /// Business Rules:
        /// - Only users with "STUDENT" role can initiate conversations
        /// - System automatically assigns an academic manager
        /// - Conversation name is set to "Liên hệ tư vấn lộ trình học"
        /// </remarks>
        /// <param name="studentId">The ID of the student who wants to create a conversation</param>
        /// <returns>Created conversation details with participants</returns>
        /// <response code="201">Conversation created successfully</response>
        /// <response code="400">Invalid student ID or user is not a student</response>
        /// <response code="500">No academic manager available or internal error</response>
        [HttpPost("create")]
        [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateConversation([FromQuery] Guid studentId)
        {
            var conversation = await _conversationService.CreateConversationAsync(studentId);
            return CreatedAtAction(nameof(GetConversation), new { id = conversation.ConversationId }, conversation);
        }
        /// <summary>
        /// Send a message in a conversation
        /// </summary>
        /// <remarks>
        /// Sends a message from an authenticated user to a specific conversation.
        /// 
        /// Security:
        /// - Sender identity is extracted from JWT token (not trusted from request body)
        /// - Only conversation participants can send messages
        /// - Message content is validated and sanitized
        /// 
        /// Process:
        /// 1. Extracts sender ID from authentication token
        /// 2. Validates conversation exists and sender is a participant
        /// 3. Validates message content
        /// 4. Creates and saves the message
        /// 5. Returns message details with sender information
        /// </remarks>
        /// <param name="conversationId">The conversation ID to send message to</param>
        /// <param name="model">Message content and details</param>
        /// <returns>Sent message details with sender information</returns>
        /// <response code="200">Message sent successfully</response>
        /// <response code="400">Invalid message content or conversation not found</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User is not a participant in the conversation</response>
        /// <response code="404">Conversation or sender not found</response>
        [HttpPost("send-messages/{conversationId}")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] MessageRequest model)
        {
            // Extract sender ID from JWT token for security
            var senderIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(senderIdStr) || !Guid.TryParse(senderIdStr, out var senderId))
            {
                            return Unauthorized(new ApiErrorResponse
            {
                StatusCode = 401,
                ErrorCode = "INVALID_TOKEN",
                Message = "Invalid user token or user not authenticated."
            });
            }

            var messageDto = await _conversationService.SendMessageAsync(conversationId, senderId, model);
            return Ok(messageDto);
        }

        /// <summary>
        /// Assign an instructor to a conversation
        /// </summary>
        /// <remarks>
        /// Adds an instructor as a participant to an existing conversation.
        /// 
        /// Process:
        /// 1. Validates conversation exists
        /// 2. Validates instructor exists and has correct role
        /// 3. Adds instructor to conversation participants if not already present
        /// 
        /// Business Rules:
        /// - Only users with "INSTRUCTOR" role can be assigned
        /// - Duplicate assignments are automatically prevented
        /// - Operation is idempotent (assigning already assigned instructor has no effect)
        /// </remarks>
        /// <param name="conversationId">The conversation ID to assign instructor to</param>
        /// <param name="instructorId">The instructor ID to assign</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Instructor assigned successfully</response>
        /// <response code="400">Invalid instructor or instructor role</response>
        /// <response code="404">Conversation or instructor not found</response>
        [HttpPost("assign-instructor/{conversationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> AssignInstructor([FromRoute] Guid conversationId, [FromQuery] Guid instructorId)
        {
            await _conversationService.AssignInstructorAsync(conversationId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Get conversation details with messages
        /// </summary>
        /// <remarks>
        /// Retrieves complete conversation information including:
        /// - Conversation metadata (ID, name, creation date)
        /// - List of participants
        /// - All messages in chronological order
        /// - Sender information for each message
        /// 
        /// Use Cases:
        /// - Displaying conversation history
        /// - Loading chat interface
        /// - Administrative review of conversations
        /// </remarks>
        /// <param name="id">The conversation ID to retrieve</param>
        /// <returns>Complete conversation details with messages and participants</returns>
        /// <response code="200">Conversation details retrieved successfully</response>
        /// <response code="404">Conversation not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversation(Guid id)
        {
            var conversation = await _conversationService.GetConversationAsync(id);
            return Ok(conversation);
        }

        /// <summary>
        /// Get all conversations for the authenticated user
        /// </summary>
        /// <remarks>
        /// Retrieves all conversations where the authenticated user is a participant.
        /// 
        /// Security:
        /// - User ID is extracted from JWT token
        /// - Only returns conversations where user is a participant
        /// 
        /// Response includes:
        /// - Conversation basic information
        /// - Participant details
        /// - Recent messages for each conversation
        /// </remarks>
        /// <returns>List of conversations for the authenticated user</returns>
        /// <response code="200">User conversations retrieved successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">User not found</response>
        [HttpGet("my-conversations")]
        [ProducesResponseType(typeof(IEnumerable<ConversationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> GetMyConversations()
        {
            // Extract user ID from JWT token
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                            return Unauthorized(new ApiErrorResponse
            {
                StatusCode = 401,
                ErrorCode = "INVALID_TOKEN",
                Message = "Invalid user token or user not authenticated."
            });
            }

            var conversations = await _conversationService.GetConversationsForUserAsync(userId);
            return Ok(conversations);
        }
    }

    
}
