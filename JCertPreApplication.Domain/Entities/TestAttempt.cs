using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class TestAttempt
    {
        public Guid attemptId { get; set; }
        public Guid userId { get; set; }
        public Guid testId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int attemptNumber { get; set; } // New, nullable
        public TestAttemptStatus status { get; set; } // New, nullable

        public int? totalScore { get; set; } // Now nullable
        public int? languageKnowledgeScore { get; set; } // Now nullable
        public int? readingScore { get; set; } // Now nullable
        public int? listeningScore { get; set; } // Now nullable
        public bool? isPass { get; set; } // Now nullable

        public virtual User User { get; set; } = null!;
        public virtual Test Test { get; set; } = null!;
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
    }
}
