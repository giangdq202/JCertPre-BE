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
        [Key] // Khóa chính
        public Guid paymentId { get; set; }

        [Required] // Khóa ngoại liên kết với Users
        [ForeignKey("User")]
        public Guid userId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Định dạng Decimal với độ chính xác 18,2
        public decimal amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string currency { get; set; }

        [Required]
        [MaxLength(50)]
        public string paymentMethod { get; set; }

        [Required]
        [MaxLength(100)]
        public string transactionId { get; set; }

        [Required]
        public PaymentStatus status { get; set; } // Sử dụng enum cho trạng thái

        [Required]
        public DateTime createdAt { get; set; }

        [MaxLength(255)]
        public string description { get; set; }

        // Navigation properties
        public virtual User User { get; set; } 
    }
}
