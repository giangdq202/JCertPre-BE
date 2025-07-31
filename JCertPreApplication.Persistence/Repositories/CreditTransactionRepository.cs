using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class CreditTransactionRepository : GenericRepository<CreditTransaction>, ICreditTransactionRepository
    {
        public CreditTransactionRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CreditTransaction>> GetUserTransactionsAsync(Guid userId)
        {
            return await _context.CreditTransactions
                .Include(ct => ct.User)
                .Where(ct => ct.user_id == userId)
                .OrderByDescending(ct => ct.created_at)
                .ToListAsync();
        }

        public async Task<int> GetUserCurrentBalanceAsync(Guid userId)
        {
            var latestTransaction = await _context.CreditTransactions
                .Where(ct => ct.user_id == userId)
                .OrderByDescending(ct => ct.created_at)
                .FirstOrDefaultAsync();

            return latestTransaction?.balance_after ?? 0;
        }

        public async Task<IEnumerable<CreditTransaction>> GetUserTransactionsByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate)
        {
            return await _context.CreditTransactions
                .Include(ct => ct.User)
                .Where(ct => ct.user_id == userId && 
                            ct.created_at >= fromDate && 
                            ct.created_at <= toDate)
                .OrderByDescending(ct => ct.created_at)
                .ToListAsync();
        }

        public async Task<IEnumerable<CreditTransaction>> GetTransactionsByTypeAsync(Guid userId, bool isCredit)
        {
            return await _context.CreditTransactions
                .Include(ct => ct.User)
                .Where(ct => ct.user_id == userId && 
                            (isCredit ? ct.amount > 0 : ct.amount < 0))
                .OrderByDescending(ct => ct.created_at)
                .ToListAsync();
        }
    }
}
