using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Payment;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JCertPreApplication.Application.Features.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICreditTransactionRepository _creditTransactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ICreditTransactionRepository creditTransactionRepository,
            IUserRepository userRepository,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _creditTransactionRepository = creditTransactionRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> HasSufficientCreditAsync(Guid userId, decimal requiredAmount)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var creditRequired = (int)Math.Ceiling(requiredAmount);
            return user.credit >= creditRequired;
        }

        public async Task<PaymentResult> ProcessCreditPaymentAsync(Guid userId, Guid courseId, decimal amount, string description)
        {
            _logger.LogInformation("Processing credit payment for user {UserId}, amount {Amount}", userId, amount);

            // Retry logic for concurrency conflicts
            const int maxRetries = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // 1. Get fresh user data for each attempt
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user == null)
                        throw ApiException.NotFound("USER_NOT_FOUND", "User not found");

                    var creditRequired = (int)Math.Ceiling(amount);
                    
                    // 2. Check sufficient credit
                    if (user.credit < creditRequired)
                    {
                        return new PaymentResult
                        {
                            IsSuccess = false,
                            Message = $"Insufficient credit. Required: {creditRequired}, Available: {user.credit}",
                            RemainingCredit = user.credit
                        };
                    }

                    // 3. Generate transaction ID
                    var transactionId = $"PAY_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N")[..8]}";

                    // 4. Store balance before transaction
                    var balanceBefore = user.credit;
                    var balanceAfter = balanceBefore - creditRequired;

                    // 5. Update user credit first (most critical operation)
                    user.credit = balanceAfter;
                    await _userRepository.UpdateAsync(user);
                    
                    // 6. Save user changes first to establish the lock
                    await _userRepository.SaveChangesAsync();

                    // 7. Create Payment record with Completed status
                    var payment = new Domain.Entities.Payment
                    {
                        paymentId = Guid.NewGuid(),
                        userId = userId,
                        amount = amount,
                        PaymentType = PaymentType.Credit,
                        transactionId = transactionId,
                        status = PaymentStatus.Completed,
                        createdAt = DateTime.UtcNow,
                        description = description
                    };

                    // 8. Create CreditTransaction record (negative amount for debit)
                    var creditTransaction = new CreditTransaction
                    {
                        transaction_id = Guid.NewGuid(),
                        user_id = userId,
                        amount = -creditRequired, // Negative for debit
                        balance_before = balanceBefore,
                        balance_after = balanceAfter,
                        description = description,
                        created_at = DateTime.UtcNow
                    };

                    // 9. Insert audit records
                    await _paymentRepository.InsertAsync(payment);
                    await _creditTransactionRepository.InsertAsync(creditTransaction);

                    // 10. Save audit records
                    await _paymentRepository.SaveChangesAsync();

                    _logger.LogInformation("Credit payment processed successfully on attempt {Attempt}. Payment ID: {PaymentId}, Transaction ID: {TransactionId}", 
                        attempt, payment.paymentId, transactionId);

                    return new PaymentResult
                    {
                        IsSuccess = true,
                        PaymentId = payment.paymentId,
                        TransactionId = transactionId,
                        Message = "Payment processed successfully",
                        RemainingCredit = balanceAfter
                    };
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("modified by another user") && attempt < maxRetries)
                {
                    _logger.LogWarning("Concurrency conflict on attempt {Attempt} for user {UserId}. Retrying... Error: {Error}", 
                        attempt, userId, ex.Message);
                    
                    // Wait a short time before retry (exponential backoff)
                    await Task.Delay(100 * attempt); // 100ms, 200ms, 300ms
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing credit payment for user {UserId} on attempt {Attempt}", userId, attempt);
                    
                    if (attempt == maxRetries)
                        throw ApiException.InternalServerError("PAYMENT_PROCESSING_FAILED", "An error occurred while processing payment after multiple attempts");
                    
                    // For other exceptions, don't retry immediately
                    throw;
                }
            }

            // This should never be reached
            throw ApiException.InternalServerError("PAYMENT_PROCESSING_FAILED", "Payment processing failed after all retry attempts");
        }

        public async Task<IEnumerable<PaymentDto>> GetUserPaymentHistoryAsync(Guid userId)
        {
            var payments = await _paymentRepository.GetUserPaymentsAsync(userId);
            
            return payments.Select(p => new PaymentDto
            {
                PaymentId = p.paymentId,
                UserId = p.userId,
                Amount = p.amount,
                PaymentType = p.PaymentType,
                TransactionId = p.transactionId,
                Status = p.status,
                CreatedAt = p.createdAt,
                Description = p.description
            });
        }

        public async Task<IEnumerable<CreditTransactionDto>> GetUserCreditHistoryAsync(Guid userId)
        {
            var transactions = await _creditTransactionRepository.GetUserTransactionsAsync(userId);
            
            return transactions.Select(ct => new CreditTransactionDto
            {
                TransactionId = ct.transaction_id,
                UserId = ct.user_id,
                Amount = ct.amount,
                BalanceBefore = ct.balance_before,
                BalanceAfter = ct.balance_after,
                Description = ct.description,
                CreatedAt = ct.created_at
            });
        }
    }
}
