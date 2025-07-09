using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Conversation;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Features.Conversation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages conversations between students and academic managers.
    /// </summary>
    [Route("api/conversation")]
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
        /// Creates a new conversation between a student and an academic manager.
        /// </summary>
        /// <param name="studentId">The ID of the student.</param>
        /// <returns>Created conversation details.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateConversation([FromQuery] Guid studentId)
        {
            var conversation = await _conversationService.CreateConversationAsync(studentId);
            return CreatedAtAction(nameof(GetConversation), new { id = conversation.ConversationId }, conversation);
        }

        /// <summary>
        /// Sends a message in a conversation.
        /// </summary>
        /// <param name="conversationId">Target conversation ID.</param>
        /// <param name="model">Message content.</param>
        /// <returns>Sent message details.</returns>
        [HttpPost("send-messages/{conversationId}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] MessageRequest model)
        {
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
        /// Assigns an instructor to a conversation.
        /// </summary>
        /// <param name="conversationId">Target conversation ID.</param>
        /// <param name="instructorId">Instructor to assign.</param>
        /// <returns>No content on success.</returns>
        [HttpPost("assign-instructor/{conversationId}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> AssignInstructor([FromRoute] Guid conversationId, [FromQuery] Guid instructorId)
        {
            await _conversationService.AssignInstructorAsync(conversationId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Gets conversation details with messages.
        /// </summary>
        /// <param name="id">Conversation ID.</param>
        /// <returns>Complete conversation details.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversation(Guid id)
        {
            var conversation = await _conversationService.GetConversationAsync(id);
            return Ok(conversation);
        }

        /// <summary>
        /// Gets all conversations for the authenticated user.
        /// </summary>
        /// <returns>List of user's conversations.</returns>
        [HttpGet("my-conversations")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> GetMyConversations()
        {
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
