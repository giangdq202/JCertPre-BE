namespace JCertPreApplication.Domain.Entities
{
    public class TestAttempt
    {
        public Guid attemptId { get; set; }
        public Guid userId { get; set; }
        public Guid testId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int totalScore { get; set; }
        public int languageKnowledgeScore { get; set; }
        public int readingScore { get; set; }
        public int listeningScore { get; set; }
        public bool isPass { get; set; }

        public virtual User User { get; set; }
        public virtual Test Test { get; set; }
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; }
    }
}
