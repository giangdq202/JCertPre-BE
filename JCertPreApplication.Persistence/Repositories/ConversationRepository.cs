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
        private new readonly JCertPreDatabaseContext _context;

        public ConversationRepository(JCertPreDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Conversation?> GetByIdWithDetailsAsync(Guid conversationId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.conversationId == conversationId);
        }

        public async Task<List<Conversation>> GetAllAsync()
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conversation>> GetConversationsForUserAsync(Guid userId)
        {
            return await _context.Conversations
                .Include(c => c.Participants)
                .Include(c => c.Messages)
                .Where(c => c.Participants.Any(p => p.userId == userId))
                .ToListAsync();
        }

        public new async Task InsertAsync(Conversation conversation)
        {
            if (conversation == null)
            {
                throw new ArgumentNullException(nameof(conversation));
            }

            // Đảm bảo các User trong Participants đã được truy vấn từ context
            foreach (var participant in conversation.Participants)
            {
                // Gắn User đã tồn tại từ database (nếu đã được truy vấn)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.userId == participant.userId);
                if (existingUser != null)
                {
                    _context.Entry(participant).State = EntityState.Unchanged; // Không chèn lại
                }
                else
                {
                    _context.Users.Add(participant); // Chèn mới nếu không tồn tại
                }
            }

            // Thêm Conversation mới
            await _context.Conversations.AddAsync(conversation);
        }

        public new async Task UpdateAsync(Conversation conversation)
        {
            if (conversation == null)
            {
                throw new ArgumentNullException(nameof(conversation));
            }

            _context.Conversations.Update(conversation);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid conversationId)
        {
            var conversation = await GetByIdWithDetailsAsync(conversationId);
            if (conversation != null)
            {
                _context.Conversations.Remove(conversation);
                await SaveChangesAsync();
            }
        }

        public new async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log chi tiết lỗi (nếu có logging như Serilog)
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                throw; // Ném lại để middleware xử lý
            }
        }
    }
}
