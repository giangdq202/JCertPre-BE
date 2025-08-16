namespace JCertPreApplication.Domain.Entities
{
    public class Feedback
    {
        public Guid feedbackId { get; set; }
        public Guid courseId { get; set; }
        public Guid userId { get; set; }
        public decimal rating { get; set; }
        public string? comment { get; set; } = null!;
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
