
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Domain.Entities
{
    public class TestAttempt
    {
        public Guid attemptId { get; set; }
        public Guid userId { get; set; }
        public Guid? testId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int attemptNumber { get; set; }
        public TestAttemptStatus status { get; set; }
        public bool? isPass { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Test Test { get; set; } = null!;
        public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
        public virtual ICollection<TestScoreSummary> TestScoreSummaries { get; set; } = new List<TestScoreSummary>(); // Add this
    }
}
