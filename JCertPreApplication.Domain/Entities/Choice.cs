namespace JCertPreApplication.Domain.Entities
{
    public class Choice
    {
        public Guid choiceId { get; set; }
        public Guid questionId { get; set; }
        public string choiceText { get; set; } = null!;
        public bool isCorrect { get; set; }

        // Navigation properties
        public virtual Question Question { get; set; } = null!;
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
