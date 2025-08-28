using JCertPreApplication.Application.Features.Payment;
using JCertPreApplication.Application.Dtos.Payment;
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
    [Authorize]
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
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetPaymentHistory(Guid userId)
        {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != userId)
            {
                return Forbid();
            }
            var payments = await _paymentService.GetUserPaymentHistoryAsync(userId);
            return Ok(payments);
        }

        /// <summary>
        /// Get credit transaction history for a specific user.
        /// </summary>
        /// <param name="userId">User ID to get credit history for</param>
        /// <returns>List of credit transactions</returns>
        [HttpGet("credit-history/{userId:guid}")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetCreditHistory(Guid userId)
        {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != userId)
            {
                return Forbid();
            }
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
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> CheckSufficientCredit(Guid userId, decimal amount)
        {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != userId)
            {
                return Forbid();
            }
            var hasSufficientCredit = await _paymentService.HasSufficientCreditAsync(userId, amount);
            
            return Ok(new { 
                UserId = userId,
                HasSufficientCredit = hasSufficientCredit,
                RequiredAmount = amount 
            });
        }

        /// <summary>
        /// Tạo link thanh toán PayOS để nạp credit (rate 1:1)
        /// </summary>
        /// <param name="request">Yêu cầu tạo thanh toán credit</param>
        /// <returns>Link thanh toán và thông tin đơn hàng</returns>
        [HttpPost("create-credit-purchase")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> CreateCreditPurchase([FromBody] CreateCreditPurchaseRequestDto request)
        {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != request.UserId)
            {
                return Forbid();
            }
            var result = await _paymentService.CreateCreditPurchaseAsync(request.UserId, request.CreditAmount);
            return Ok(result);
        }

        /// <summary>
        /// Webhook endpoint cho PayOS - xử lý kết quả thanh toán
        /// </summary>
        /// <param name="webhookBody">Dữ liệu webhook từ PayOS</param>
        /// <returns>200 OK response</returns>
        [HttpPost("payos-webhook")]
        [Authorize]
        public async Task<IActionResult> HandlePayOSWebhook([FromBody] WebhookTypeDto webhookBody)
        {
            try
            {
                await _paymentService.ProcessPayOSWebhookAsync(webhookBody);
                return Ok(new { message = "Webhook processed successfully" });
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng vẫn trả về 200 để PayOS không gửi lại
                Console.WriteLine($"Error processing webhook: {ex.Message}");
                return Ok(new { message = "Webhook received but processing failed" });
            }
        }

        /// <summary>
        /// Đăng ký webhook URL với PayOS (chỉ cần chạy 1 lần)
        /// </summary>
        /// <param name="request">URL webhook cần đăng ký</param>
        /// <returns>Kết quả đăng ký</returns>
        [HttpPost("confirm-webhook")]
        [Authorize]
        public async Task<IActionResult> ConfirmWebhook([FromBody] ConfirmWebhookRequestDto request)
        {
            try
            {
                var result = await _paymentService.ConfirmPayOSWebhookAsync(request.WebhookUrl);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Lỗi khi đăng ký webhook: {ex.Message}" });
            }
        }

        /// <summary>
        /// Handle payment return from PayOS (success callback)
        /// </summary>
        [HttpGet("return")]
        [Authorize]
        public async Task<IActionResult> HandlePaymentReturn([FromQuery] PaymentCallbackRequestDto request)
        {
            var result = await _paymentService.HandlePaymentReturnAsync(request);
            
            // Redirect đến frontend với kết quả
            return Redirect(result.RedirectUrl);
        }

        /// <summary>
        /// Handle payment cancellation from PayOS
        /// </summary>
        [HttpGet("cancel")]
        [Authorize]
        public async Task<IActionResult> HandlePaymentCancel([FromQuery] PaymentCallbackRequestDto request)
        {
            var result = await _paymentService.HandlePaymentCancelAsync(request);
            
            // Redirect đến frontend với kết quả
            return Redirect(result.RedirectUrl);
        }
    }
}
