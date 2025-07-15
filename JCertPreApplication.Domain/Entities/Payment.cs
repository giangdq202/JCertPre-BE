using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class Payment
    {
        public Guid paymentId { get; set; }
        public Guid userId { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; } = null!;
        public PaymentType PaymentType { get; set; }
        public string paymentMethod { get; set; } = null!;
        public string transactionId { get; set; } = null!;
        public PaymentStatus status { get; set; }
        public DateTime createdAt { get; set; }
        public string? description { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}
