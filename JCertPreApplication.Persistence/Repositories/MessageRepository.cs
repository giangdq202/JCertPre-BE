using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private new readonly JCertPreDatabaseContext _context;
        public MessageRepository(JCertPreDatabaseContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public new async Task InsertAsync(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Đảm bảo các User trong Participants đã được truy vấn từ context
            

            // Thêm message mới
            await _context.Messages.AddAsync(message);
        }
    }
}
