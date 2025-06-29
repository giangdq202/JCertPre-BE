using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<Conversation> GetByIdWithDetailsAsync(Guid conversationId)
        {
            return await _dbSet
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.conversationId == conversationId);
        }
    }
}
