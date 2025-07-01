using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Features.Conversation;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/conversation")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateConversation([FromQuery] Guid studentId)
        {
            var conversation = await _conversationService.CreateConversationAsync(studentId);
            var result = new
            {
                conversation.conversationId,
                conversation.conversationName,
                conversation.createdAt
            };
            return Ok(result);
        }
        [HttpPost("send-messages/{conversationId}")]
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] MessageRequest model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                return BadRequest("Message content is required.");
            }

            try
            {
                var message = await _conversationService.SendMessageAsync(conversationId, model);
                var result = new
                {
                    messageId = message.messageId,
                    content = message.content,
                    conversationId = message.conversationId,
                    senderId = message.senderId,
                    sentAt = message.sentAt
                };
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("assign-instructor/{conversationId}")]
        public async Task<IActionResult> AssignInstructor([FromRoute] Guid conversationId, [FromQuery] Guid instructorId)
        {
            try
            {
                await _conversationService.AssignInstructorAsync(conversationId, instructorId);
                return Ok(new { message = "Instructor assigned successfully." });
            }
            catch (FormatException ex)
            {
                return BadRequest($"Invalid GUID format: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("conversation-message/{conversationId}")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var conversation = await _conversationService.GetConversationAsync(conversationId);
            if (conversation == null)
            {
                return NotFound();
            }
            var result = new
            {
                conversationId = conversation.conversationId,
                conversationName = conversation.conversationName,
                createdAt = conversation.createdAt,
                messages = conversation.Messages.Select(m => new
                {
                    messageId = m.messageId,
                    content = m.content,
                    senderId = m.senderId,
                    sentAt = m.sentAt
                })
            };
            return Ok(result);
        }
        [HttpGet("my-messages")]
        public async Task<IActionResult> GetMyMessages(Guid userId)
        {
            var messages = await _conversationService.GetMyMessagesAsync(userId);
            return Ok(messages);
        }
    }

    
}
