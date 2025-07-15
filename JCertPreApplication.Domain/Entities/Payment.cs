using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Payment
    {
        public Guid paymentId { get; set; }
        public Guid userId { get; set; }
        public decimal amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public string? transactionId { get; set; }
        public PaymentStatus status { get; set; }
        public DateTime createdAt { get; set; }
        public string? description { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}
