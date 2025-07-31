using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Dtos.Payment
{
    /// <summary>
    /// Result of payment processing operation
    /// </summary>
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public Guid? PaymentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RemainingCredit { get; set; }
        public string? TransactionId { get; set; }
    }

    /// <summary>
    /// Payment data transfer object
    /// </summary>
    public class PaymentDto
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string? TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Credit transaction data transfer object
    /// </summary>
    public class CreditTransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Request DTO for processing credit payment
    /// </summary>
    public class ProcessCreditPaymentRequest
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
