using JCertPreApplication.Application.Dtos.Payment;

namespace JCertPreApplication.Application.Features.Payment
{
    public interface IPaymentService
    {
        /// <summary>
        /// Process credit payment for course enrollment
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="description">Payment description</param>
        /// <returns>Payment result</returns>
        Task<PaymentResult> ProcessCreditPaymentAsync(Guid userId, Guid courseId, decimal amount, string description);

        /// <summary>
        /// Check if user has sufficient credit
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="requiredAmount">Required amount</param>
        /// <returns>True if has sufficient credit</returns>
        Task<bool> HasSufficientCreditAsync(Guid userId, decimal requiredAmount);

        /// <summary>
        /// Get user payment history
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of payment DTOs</returns>
        Task<IEnumerable<PaymentDto>> GetUserPaymentHistoryAsync(Guid userId);

        /// <summary>
        /// Get user credit transaction history
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of credit transaction DTOs</returns>
        Task<IEnumerable<CreditTransactionDto>> GetUserCreditHistoryAsync(Guid userId);
    }
}
