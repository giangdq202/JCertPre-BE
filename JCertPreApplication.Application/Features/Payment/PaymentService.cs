using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Payment;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JCertPreApplication.Application.Features.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICreditTransactionRepository _creditTransactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentGateway _paymentGateway;
        private readonly FrontendConfiguration _frontendConfig;

        public PaymentService(
            IPaymentRepository paymentRepository,
            ICreditTransactionRepository creditTransactionRepository,
            IUserRepository userRepository,
            ILogger<PaymentService> logger,
            IPaymentGateway paymentGateway,
            IOptions<FrontendConfiguration> frontendConfig)
        {
            _paymentRepository = paymentRepository;
            _creditTransactionRepository = creditTransactionRepository;
            _userRepository = userRepository;
            _logger = logger;
            _paymentGateway = paymentGateway;
            _frontendConfig = frontendConfig.Value;
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

            // 2. Đơn giản hóa orderCode - PayOS chấp nhận long đơn giản
            var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // 3. Tính amount (rate 1:1 - 1 credit = 1 VND)
            var amount = creditAmount;

            // 4. Tạo description ngắn gọn cho PayOS (tối đa 25 ký tự)
            var shortDescription = $"Nap {creditAmount} credit";
            if (shortDescription.Length > 25)
            {
                shortDescription = "Nap credit";
            }

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
        /// Xử lý PayOS webhook - dùng như BACKUP mechanism khi return URL processing bị lỗi
        /// Primary flow vẫn là HandlePaymentReturnAsync, webhook đảm bảo không miss payment nào
        /// </summary>
        public async Task ProcessPayOSWebhookAsync(WebhookTypeDto webhookBody)
        {
            // 1. Xác thực webhook
            var verifiedData = _paymentGateway.VerifyPaymentWebhookData(webhookBody);

            _logger.LogInformation("Received PayOS webhook for orderCode {OrderCode} with status {Status}", 
                verifiedData.OrderCode, webhookBody.Success ? "SUCCESS" : "FAILED");

            // 2. Tìm payment theo transactionId (orderCode)
            var payment = await _paymentRepository.GetByTransactionIdAsync(verifiedData.OrderCode.ToString());
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for orderCode: {OrderCode}", verifiedData.OrderCode);
                return;
            }

            // 3. Kiểm tra nếu đã xử lý rồi (idempotency)
            if (payment.status != PaymentStatus.Pending)
            {
                _logger.LogInformation("Payment {PaymentId} already processed with status {Status}", 
                    payment.paymentId, payment.status);
                return;
            }

            // 4. Xử lý theo trạng thái webhook
            if (webhookBody.Success && verifiedData.Code == "00")
            {
                // Thanh toán thành công
                await ProcessPaymentSuccessAsync(payment, verifiedData, $"Nap {(int)payment.amount} credit qua PayOS - Order: {verifiedData.OrderCode}");
            }
            else
            {
                // Thanh toán thất bại - inline ProcessFailedPayment
                payment.status = PaymentStatus.Failed;
                payment.description = $"{payment.description} - Failed: {webhookBody.Desc}";
                
                await _paymentRepository.UpdateAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation("Payment {PaymentId} marked as failed. Reason: {Reason}",
                    payment.paymentId, webhookBody.Desc);
            }
        }

        /// <summary>
        /// Xử lý payment thành công - dùng chung cho webhook và return URL
        /// </summary>
        private async Task ProcessPaymentSuccessAsync(Domain.Entities.Payment payment, object paymentData, string transactionDescription)
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
                    description = transactionDescription,
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

        public async Task<string> ConfirmPayOSWebhookAsync(string webhookUrl)
        {
            return await _paymentGateway.ConfirmWebhookAsync(webhookUrl);
        }

        /// <summary>
        /// Xử lý khi user quay lại từ PayOS sau khi thanh toán - PRIMARY PAYMENT PROCESSING FLOW
        /// Method này call PayOS API trực tiếp để verify và process payment real-time
        /// Webhook sẽ serve như backup mechanism phòng trường hợp return URL processing bị lỗi
        /// </summary>
        public async Task<PaymentCallbackResponseDto> HandlePaymentReturnAsync(PaymentCallbackRequestDto request)
        {
            _logger.LogInformation("PayOS Return - OrderCode: {OrderCode}, Status: {Status}, Code: {Code}, Cancel: {Cancel}", 
                request.OrderCode, request.Status, request.Code, request.Cancel);

            try
            {
                // 1. Handle cancellation first (từ query params)
                if (request.Cancel || request.Status == "CANCELLED")
                {
                    return await HandlePaymentCancelLogic(request.OrderCode);
                }

                // 2. Handle error codes từ PayOS
                if (!string.IsNullOrEmpty(request.Code) && request.Code != "00")
                {
                    _logger.LogWarning("PayOS returned error code {Code} for order {OrderCode}", request.Code, request.OrderCode);
                    
                    return new PaymentCallbackResponseDto
                    {
                        Success = false,
                        Message = $"Payment failed with code: {request.Code}",
                        RedirectUrl = $"{_frontendConfig.PaymentErrorUrl}?code={request.Code}&orderId={request.OrderCode}"
                    };
                }

                // 3. Handle success case (từ query params)
                if (request.Status == "PAID" && request.Code == "00")
                {
                    return await HandlePaymentSuccessLogic(request);
                }

                // 4. Handle other statuses (PENDING, etc.)
                return new PaymentCallbackResponseDto
                {
                    Success = false,
                    Message = $"Payment status: {request.Status}",
                    RedirectUrl = $"{_frontendConfig.PaymentPendingUrl}?orderId={request.OrderCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment return for orderCode {OrderCode}", request.OrderCode);
                return new PaymentCallbackResponseDto
                {
                    Success = false,
                    Message = "Internal server error",
                    RedirectUrl = $"{_frontendConfig.PaymentErrorUrl}?reason=server-error"
                };
            }
        }

        /// <summary>
        /// Xử lý logic khi payment thành công từ return URL
        /// </summary>
        private async Task<PaymentCallbackResponseDto> HandlePaymentSuccessLogic(PaymentCallbackRequestDto request)
        {
            var payment = await _paymentRepository.GetByTransactionIdAsync(request.OrderCode.ToString());
            
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for orderCode: {OrderCode}", request.OrderCode);
                return new PaymentCallbackResponseDto
                {
                    Success = false,
                    Message = "Payment not found",
                    RedirectUrl = $"{_frontendConfig.PaymentErrorUrl}?reason=not-found"
                };
            }

            // Nếu đã completed, redirect thẳng
            if (payment.status == PaymentStatus.Completed)
            {
                _logger.LogInformation("Payment {PaymentId} already completed", payment.paymentId);
                return new PaymentCallbackResponseDto
                {
                    Success = true,
                    Message = "Payment already completed",
                    RedirectUrl = $"{_frontendConfig.PaymentSuccessUrl}?orderId={request.OrderCode}"
                };
            }

            // Verify thêm với PayOS API để đảm bảo
            var paymentInfo = await _paymentGateway.GetPaymentLinkInformationAsync(request.OrderCode);
            
            if (paymentInfo.Status == "PAID")
            {
                await ProcessPaymentSuccessAsync(payment, paymentInfo, 
                    $"Nap {(int)payment.amount} credit qua PayOS Return - Order: {request.OrderCode}");
            }

            return new PaymentCallbackResponseDto
            {
                Success = true,
                Message = "Payment processed successfully",
                RedirectUrl = $"{_frontendConfig.PaymentSuccessUrl}?orderId={request.OrderCode}"
            };
        }

        /// <summary>
        /// Xử lý logic khi payment bị cancel từ return URL
        /// </summary>
        private async Task<PaymentCallbackResponseDto> HandlePaymentCancelLogic(long orderCode)
        {
            _logger.LogInformation("Processing payment cancellation for orderCode {OrderCode}", orderCode);

            var payment = await _paymentRepository.GetByTransactionIdAsync(orderCode.ToString());
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for orderCode: {OrderCode}", orderCode);
                return new PaymentCallbackResponseDto
                {
                    Success = true, // Still redirect to cancel page
                    Message = "Payment not found",
                    RedirectUrl = $"{_frontendConfig.PaymentCancelledUrl}?reason=not-found"
                };
            }

            // Chỉ cancel nếu đang Pending
            if (payment.status == PaymentStatus.Pending)
            {
                // Cancel payment link trên PayOS (optional)
                try
                {
                    await _paymentGateway.CancelPaymentLinkAsync(orderCode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to cancel PayOS payment link {OrderCode}", orderCode);
                    // Không fail toàn bộ process
                }

                // Update payment status
                payment.status = PaymentStatus.Failed;
                payment.description = $"{payment.description} - Cancelled by user at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                
                await _paymentRepository.UpdateAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation("Payment {PaymentId} cancelled by user", payment.paymentId);
            }

            return new PaymentCallbackResponseDto
            {
                Success = true,
                Message = "Payment cancelled successfully",
                RedirectUrl = $"{_frontendConfig.PaymentCancelledUrl}?orderId={orderCode}"
            };
        }

        /// <summary>
        /// Xử lý khi user cancel payment từ PayOS
        /// </summary>
        public async Task<PaymentCallbackResponseDto> HandlePaymentCancelAsync(PaymentCallbackRequestDto request)
        {
            _logger.LogInformation("Handling payment cancel request for orderCode {OrderCode}", request.OrderCode);

            try
            {
                return await HandlePaymentCancelLogic(request.OrderCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment cancellation for orderCode {OrderCode}", request.OrderCode);
                return new PaymentCallbackResponseDto
                {
                    Success = true, // Still redirect even on error
                    Message = "Payment cancelled with errors",
                    RedirectUrl = $"{_frontendConfig.PaymentCancelledUrl}?reason=error"
                };
            }
        }
    }
}
