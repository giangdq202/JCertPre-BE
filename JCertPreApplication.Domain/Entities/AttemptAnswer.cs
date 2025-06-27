namespace JCertPreApplication.Domain.Entities
{
    public class AttemptAnswer
    {
        public Guid answerId { get; set; }
        public Guid attemptId { get; set; }
        public Guid questionId { get; set; }
        public Guid choiceId { get; set; }

        // Navigation properties
        public virtual TestAttempt TestAttempt { get; set; }
        public virtual Question Question { get; set; }
        public virtual Choice Choice { get; set; }
    }
}
