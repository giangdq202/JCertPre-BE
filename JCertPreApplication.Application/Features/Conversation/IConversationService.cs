using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Domain.Entities;
namespace JCertPreApplication.Application.Features.Conversation
{
    public interface IConversationService
    {
        
        Task<Domain.Entities.Conversation> CreateConversationAsync( Guid studentId);


        Task<Message> SendMessageAsync(Guid conversationId, MessageRequest messageRequest);



        Task AssignInstructorAsync(Guid conversationId, Guid instructorId);

        
        Task<Domain.Entities.Conversation> GetConversationAsync(Guid conversationId);
        Task<List<Message>> GetMyMessagesAsync(Guid userId);
    }
}
