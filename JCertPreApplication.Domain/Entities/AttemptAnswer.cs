namespace JCertPreApplication.Domain.Entities
{
    public class AttemptAnswer
    {
        public Guid answerId { get; set; }
        public Guid attemptId { get; set; }
        public Guid? questionId { get; set; }
        public Guid? choiceId { get; set; }
        public bool isCorrect { get; set; } // New field
        public int score { get; set; }      // New field

        // Navigation properties
        public virtual TestAttempt TestAttempt { get; set; } = null!;
        public virtual Question? Question { get; set; } = null!;
        public virtual Choice? Choice { get; set; } = null!;
    }
}
