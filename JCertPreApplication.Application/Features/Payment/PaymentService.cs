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
        private readonly IPaymentGateway _paymentGateway;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ICreditTransactionRepository creditTransactionRepository,
            IUserRepository userRepository,
            ILogger<PaymentService> logger,
            IPaymentGateway paymentGateway)
        {
            _paymentRepository = paymentRepository;
            _creditTransactionRepository = creditTransactionRepository;
            _userRepository = userRepository;
            _logger = logger;
            _paymentGateway = paymentGateway;
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

        public async Task<CreateCreditPurchaseResponseDto> CreateCreditPurchaseAsync(Guid userId, int creditAmount)
        {
            // 1. Kiểm tra user tồn tại
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw ApiException.NotFound("USER_NOT_FOUND", "User not found");

            // 2. Tạo orderCode duy nhất (PayOS yêu cầu long)
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var userIdShort = Math.Abs(userId.GetHashCode()) % 1000000; // Lấy 6 chữ số cuối
            var orderCode = long.Parse($"{timestamp}{userIdShort:D6}"[..15]); // Giới hạn 15 chữ số

            // 3. Tính amount (rate 1:1 - 1 credit = 1 VND)
            var amount = creditAmount;

            // 4. Tạo description ngắn gọn cho PayOS (tối đa 25 ký tự)
            var shortDescription = CreatePayOSDescription(creditAmount);

            // 5. Tạo PaymentDataDto
            var paymentData = new PaymentDataDto
            {
                OrderCode = orderCode,
                Amount = amount,
                Description = shortDescription, // Rút ngắn để <= 25 ký tự
                Items = new List<ItemDataDto>
                {
                    new ItemDataDto
                    {
                        Name = "Credit Package",
                        Quantity = 1,
                        Price = amount
                    }
                }
                // ReturnUrl và CancelUrl sẽ được set trong PayOSService
            };

            // 6. Tạo bản ghi Payment với trạng thái Pending
            var payment = new Domain.Entities.Payment
            {
                paymentId = Guid.NewGuid(),
                userId = userId,
                amount = amount,
                PaymentType = PaymentType.Money, // PayOS vẫn là Money type
                transactionId = orderCode.ToString(),
                status = PaymentStatus.Pending,
                createdAt = DateTime.UtcNow,
                description = $"Nap {creditAmount} credit cho {user.fullName}" // Chi tiết hơn cho Payment record
            };

            await _paymentRepository.InsertAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            // 7. Tạo link thanh toán qua PayOS
            var result = await _paymentGateway.CreatePaymentLinkAsync(paymentData);

            _logger.LogInformation("Created credit purchase payment for user {UserId}, amount {Amount}, orderCode {OrderCode}", 
                userId, amount, orderCode);

            return new CreateCreditPurchaseResponseDto
            {
                PaymentUrl = result.CheckoutUrl,
                OrderCode = orderCode,
                Amount = amount,
                Description = shortDescription // "Nap {creditAmount} credit"
            };
        }

        /// <summary>
        /// Tạo description ngắn gọn cho PayOS (tối đa 25 ký tự)
        /// </summary>
        private static string CreatePayOSDescription(int creditAmount)
        {
            var description = $"Nap {creditAmount} credit";
            
            // Đảm bảo không vượt quá 25 ký tự
            if (description.Length > 25)
            {
                // Nếu quá dài, rút ngắn thành "Nap credit"
                description = "Nap credit";
            }
            
            return description;
        }

        public async Task ProcessPayOSWebhookAsync(WebhookTypeDto webhookBody)
        {
            // 1. Log chi tiết webhook response
            //LogPayOSWebhookDetails(webhookBody);

            // 2. Xác thực webhook
            var verifiedData = _paymentGateway.VerifyPaymentWebhookData(webhookBody);

            _logger.LogInformation("Received PayOS webhook for orderCode {OrderCode} with status {Status}", 
                verifiedData.OrderCode, webhookBody.Success ? "SUCCESS" : "FAILED");

            // 3. Tìm payment theo transactionId (orderCode)
            var payment = await _paymentRepository.GetByTransactionIdAsync(verifiedData.OrderCode.ToString());
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for orderCode: {OrderCode}", verifiedData.OrderCode);
                return;
            }

            // 4. Kiểm tra nếu đã xử lý rồi (idempotency)
            if (payment.status != PaymentStatus.Pending)
            {
                _logger.LogInformation("Payment {PaymentId} already processed with status {Status}", 
                    payment.paymentId, payment.status);
                return;
            }

            // 5. Xử lý theo trạng thái webhook
            if (webhookBody.Success && verifiedData.Code == "00")
            {
                // Thanh toán thành công
                await ProcessSuccessfulPayment(payment, verifiedData);
            }
            else
            {
                // Thanh toán thất bại
                await ProcessFailedPayment(payment, webhookBody.Desc);
            }
        }

        /// <summary>
        /// Log chi tiết thông tin webhook từ PayOS để debug
        /// </summary>
        private void LogPayOSWebhookDetails(WebhookTypeDto webhookBody)
        {
            _logger.LogInformation("=== PayOS Webhook Details ===");
            _logger.LogInformation("Webhook Code: {Code}", webhookBody.Code);
            _logger.LogInformation("Webhook Desc: {Desc}", webhookBody.Desc);
            _logger.LogInformation("Webhook Success: {Success}", webhookBody.Success);
            _logger.LogInformation("Webhook Signature: {Signature}", webhookBody.Signature);
            
            if (webhookBody.Data != null)
            {
                _logger.LogInformation("--- Webhook Data ---");
                _logger.LogInformation("OrderCode: {OrderCode}", webhookBody.Data.OrderCode);
                _logger.LogInformation("Amount: {Amount} VND", webhookBody.Data.Amount);
                _logger.LogInformation("Description: {Description}", webhookBody.Data.Description);
                _logger.LogInformation("AccountNumber: {AccountNumber}", webhookBody.Data.AccountNumber);
                _logger.LogInformation("Reference: {Reference}", webhookBody.Data.Reference);
                _logger.LogInformation("TransactionDateTime: {TransactionDateTime}", webhookBody.Data.TransactionDateTime);
                _logger.LogInformation("Currency: {Currency}", webhookBody.Data.Currency);
                _logger.LogInformation("PaymentLinkId: {PaymentLinkId}", webhookBody.Data.PaymentLinkId);
                _logger.LogInformation("Data Code: {Code}", webhookBody.Data.Code);
                _logger.LogInformation("Data Desc: {Desc}", webhookBody.Data.Desc);
                _logger.LogInformation("CounterAccountBankId: {CounterAccountBankId}", webhookBody.Data.CounterAccountBankId);
                _logger.LogInformation("CounterAccountBankName: {CounterAccountBankName}", webhookBody.Data.CounterAccountBankName);
                _logger.LogInformation("CounterAccountName: {CounterAccountName}", webhookBody.Data.CounterAccountName);
                _logger.LogInformation("CounterAccountNumber: {CounterAccountNumber}", webhookBody.Data.CounterAccountNumber);
                _logger.LogInformation("VirtualAccountName: {VirtualAccountName}", webhookBody.Data.VirtualAccountName);
                _logger.LogInformation("VirtualAccountNumber: {VirtualAccountNumber}", webhookBody.Data.VirtualAccountNumber);
            }
            else
            {
                _logger.LogWarning("Webhook Data is NULL");
            }
            
            _logger.LogInformation("=== End PayOS Webhook Details ===");
        }

        private async Task ProcessSuccessfulPayment(Domain.Entities.Payment payment, WebhookDataDto verifiedData)
        {
            // 1. Lấy thông tin user
            var user = await _userRepository.GetByIdAsync(payment.userId);
            if (user == null)
            {
                _logger.LogError("User not found for payment {PaymentId}", payment.paymentId);
                return;
            }

            // 2. Tính số credit cần thêm (rate 1:1)
            var creditToAdd = (int)payment.amount;
            var balanceBefore = user.credit;
            var balanceAfter = balanceBefore + creditToAdd;

            try
            {
                // 3. Cập nhật credit user
                user.credit = balanceAfter;
                await _userRepository.UpdateAsync(user);

                // 4. Cập nhật trạng thái payment
                payment.status = PaymentStatus.Completed;
                await _paymentRepository.UpdateAsync(payment);

                // 5. Tạo credit transaction record
                var creditTransaction = new CreditTransaction
                {
                    transaction_id = Guid.NewGuid(),
                    user_id = payment.userId,
                    amount = creditToAdd, // Positive cho deposit
                    balance_before = balanceBefore,
                    balance_after = balanceAfter,
                    description = $"Nap {creditToAdd} credit qua PayOS - Order: {verifiedData.OrderCode}",
                    created_at = DateTime.UtcNow
                };

                await _creditTransactionRepository.InsertAsync(creditTransaction);
                
                // 6. Save tất cả changes
                await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully processed PayOS payment {PaymentId} for user {UserId}. Added {Credit} credit. Balance: {BalanceBefore} -> {BalanceAfter}",
                    payment.paymentId, payment.userId, creditToAdd, balanceBefore, balanceAfter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing successful payment {PaymentId}", payment.paymentId);
                throw;
            }
        }

        private async Task ProcessFailedPayment(Domain.Entities.Payment payment, string reason)
        {
            // Cập nhật trạng thái thất bại
            payment.status = PaymentStatus.Failed;
            payment.description = $"{payment.description} - Failed: {reason}";
            
            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} marked as failed. Reason: {Reason}",
                payment.paymentId, reason);
        }

        public async Task<string> ConfirmPayOSWebhookAsync(string webhookUrl)
        {
            return await _paymentGateway.ConfirmWebhookAsync(webhookUrl);
        }
    }
}
