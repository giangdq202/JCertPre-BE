using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(Guid userId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.userId == userId)
                .OrderByDescending(p => p.createdAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.Payments
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.transactionId == transactionId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.status == status)
                .OrderByDescending(p => p.createdAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Where(p => p.userId == userId && 
                           p.createdAt >= fromDate && 
                           p.createdAt <= toDate)
                .OrderByDescending(p => p.createdAt)
                .ToListAsync();
        }
    }
}
