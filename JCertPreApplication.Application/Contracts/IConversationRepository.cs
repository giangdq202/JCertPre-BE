using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        Task<Conversation?> GetByIdWithDetailsAsync(Guid conversationId);

        Task<List<Conversation>> GetAllAsync();

        Task<IEnumerable<Conversation>> GetConversationsForUserAsync(Guid userId);

        new Task InsertAsync(Conversation conversation);

        new Task UpdateAsync(Conversation conversation);

        Task DeleteAsync(Guid conversationId);

        new Task SaveChangesAsync();
    }
}
