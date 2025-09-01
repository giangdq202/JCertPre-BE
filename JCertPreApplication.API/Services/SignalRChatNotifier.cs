using System;
using System.Threading.Tasks;
using JCertPreApplication.API.Hubs;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Conversation;
using Microsoft.AspNetCore.SignalR;

namespace JCertPreApplication.API.Services
{
    public class SignalRChatNotifier : IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRChatNotifier(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMessageCreatedAsync(Guid conversationId, MessageDto message)
        {
            return _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("MessageCreated", message);
        }
    }
}


