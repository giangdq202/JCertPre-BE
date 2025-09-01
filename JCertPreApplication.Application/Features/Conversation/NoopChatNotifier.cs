using System;
using System.Threading.Tasks;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Conversation;

namespace JCertPreApplication.Application.Features.Conversation
{
    internal class NoopChatNotifier : IChatNotifier
    {
        public Task NotifyMessageCreatedAsync(Guid conversationId, MessageDto message)
        {
            return Task.CompletedTask;
        }
    }
}


