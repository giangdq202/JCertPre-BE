using System;
using System.Security.Claims;
using System.Threading.Tasks;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Conversation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace JCertPreApplication.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationService _conversationService;

        public ChatHub(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public async Task JoinConversation(Guid conversationId)
        {
            // Security: verify current user is participant before joining group
            var conversation = await _conversationService.GetConversationAsync(conversationId);
            var userIdString = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (conversation == null || string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                throw new HubException("UNAUTHORIZED");
            }
            var isParticipant = conversation.Participants.Exists(p => p.Id == userId);
            if (!isParticipant)
            {
                throw new HubException("FORBIDDEN");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        public async Task LeaveConversation(Guid conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
        }
    }
}


