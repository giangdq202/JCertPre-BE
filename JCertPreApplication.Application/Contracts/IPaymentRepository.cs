using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        /// <summary>
        /// Get all payments for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user payments</returns>
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(Guid userId);

        /// <summary>
        /// Get payment by transaction ID
        /// </summary>
        /// <param name="transactionId">Transaction ID</param>
        /// <returns>Payment record</returns>
        Task<Payment?> GetByTransactionIdAsync(string transactionId);

        /// <summary>
        /// Get payments by status
        /// </summary>
        /// <param name="status">Payment status</param>
        /// <returns>List of payments with specific status</returns>
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(Domain.Enums.PaymentStatus status);

        /// <summary>
        /// Get user payments within date range
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="fromDate">From date</param>
        /// <param name="toDate">To date</param>
        /// <returns>List of payments in date range</returns>
        Task<IEnumerable<Payment>> GetUserPaymentsByDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get all payments by payment type
        /// </summary>
        /// <param name="paymentType">Payment type</param>
        /// <returns>List of payments with specific type</returns>
        Task<IEnumerable<Payment>> GetPaymentsByTypeAsync(Domain.Enums.PaymentType paymentType);
    }
}
