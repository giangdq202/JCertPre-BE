using JCertPreApplication.Application.Features.Conversation;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ConversationService _conversationService;

        public ConversationController(ConversationService conversationService)
        {
            _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation([FromQuery] Guid studentId)
        {
            var conversation = await _conversationService.CreateConversationAsync(studentId);
            return Ok(conversation);
        }
        [HttpPost("{conversationId}/messages")]
        public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] MessageRequest model)
        {
            if (string.IsNullOrEmpty(model.Content))
            {
                return BadRequest("Message content is required.");
            }

            var message = await _conversationService.SendMessageAsync(conversationId, model.Content);
            return Ok(message);
        }

        [HttpPost("{conversationId}/assign")]
        public async Task<IActionResult> AssignInstructor(Guid conversationId, [FromQuery] Guid instructorId)
        {
            await _conversationService.AssignInstructorAsync(conversationId, instructorId);
            return Ok();
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var conversation = await _conversationService.GetConversationAsync(conversationId);
            if (conversation == null)
            {
                return NotFound();
            }
            return Ok(conversation.Messages);
        }
    }

    public class MessageRequest
    {
        public string Content { get; set; }
    }
}
