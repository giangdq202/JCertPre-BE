using System;

namespace JCertPreApplication.Domain.Entities
{
    public class CreditTransaction
    {
        public Guid transaction_id { get; set; }
        public Guid user_id { get; set; }
        public int amount { get; set; }
        public int balance_before { get; set; }
        public int balance_after { get; set; }
        public string description { get; set; } = null!;
        public DateTime created_at { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}
