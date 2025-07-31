using JCertPreApplication.Application.Features.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles payment operations including credit transactions and payment history.
    /// </summary>
    [Route("api/payment")]
    [ApiController]
    [Tags("Payment")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        /// <summary>
        /// Get payment history for a specific user.
        /// </summary>
        /// <param name="userId">User ID to get payment history for</param>
        /// <returns>List of user payments</returns>
        [HttpGet("history/{userId:guid}")]
        public async Task<IActionResult> GetPaymentHistory(Guid userId)
        {
            var payments = await _paymentService.GetUserPaymentHistoryAsync(userId);
            return Ok(payments);
        }

        /// <summary>
        /// Get credit transaction history for a specific user.
        /// </summary>
        /// <param name="userId">User ID to get credit history for</param>
        /// <returns>List of credit transactions</returns>
        [HttpGet("credit-history/{userId:guid}")]
        public async Task<IActionResult> GetCreditHistory(Guid userId)
        {
            var transactions = await _paymentService.GetUserCreditHistoryAsync(userId);
            return Ok(transactions);
        }

        /// <summary>
        /// Check if a user has sufficient credit for a specific amount.
        /// </summary>
        /// <param name="userId">User ID to check credit for</param>
        /// <param name="amount">Amount to check</param>
        /// <returns>Boolean indicating if user has sufficient credit</returns>
        [HttpGet("check-credit/{userId:guid}/{amount:decimal}")]
        public async Task<IActionResult> CheckSufficientCredit(Guid userId, decimal amount)
        {
            var hasSufficientCredit = await _paymentService.HasSufficientCreditAsync(userId, amount);
            
            return Ok(new { 
                UserId = userId,
                HasSufficientCredit = hasSufficientCredit,
                RequiredAmount = amount 
            });
        }
    }
}
