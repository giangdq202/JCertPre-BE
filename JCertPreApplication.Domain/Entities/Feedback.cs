namespace JCertPreApplication.Domain.Entities
{
    public class Feedback
    {
        public Guid feedbackId { get; set; }
        public Guid courseId { get; set; }
        public Guid userId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public string? reply { get; set; }
        public DateTime createdAt { get; set; }

        // Navigation properties
        public virtual Course Course { get; set; }
        public virtual User User { get; set; }
    }
}
