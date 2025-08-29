using System;
using System.Threading.Tasks;
using JCertPreApplication.Application.Dtos.Conversation;

namespace JCertPreApplication.Application.Contracts
{
    public interface IChatNotifier
    {
        Task NotifyMessageCreatedAsync(Guid conversationId, MessageDto message);
    }
}


