using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ICreditTransactionRepository : IGenericRepository<CreditTransaction>
    {
        /// <summary>
        /// Get all credit transactions for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user credit transactions</returns>
        Task<IEnumerable<CreditTransaction>> GetUserTransactionsAsync(Guid userId);

        /// <summary>
        /// Get user's current credit balance
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Current balance</returns>
        Task<int> GetUserCurrentBalanceAsync(Guid userId);

        /// <summary>
        /// Get user transactions within date range
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <returns>List of transactions in date range</returns>
        Task<IEnumerable<CreditTransaction>> GetUserTransactionsByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get transactions by type (positive/negative amounts)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="isCredit">True for credit (positive), false for debit (negative)</param>
        /// <returns>List of transactions by type</returns>
        Task<IEnumerable<CreditTransaction>> GetTransactionsByTypeAsync(Guid userId, bool isCredit);
    }
}
