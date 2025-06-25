using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Domain.Entities
{
    public class Payment
    {
        public Guid paymentId { get; set; }
        public Guid userId { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string paymentMethod { get; set; }
        public string transactionId { get; set; }
        public PaymentStatus status { get; set; }
        public DateTime createdAt { get; set; }
        public string description { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}
