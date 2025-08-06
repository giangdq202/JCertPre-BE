using System;

namespace JCertPreApplication.Domain.Entities
{
    /// <summary>
    /// Stores pre-calculated max scores for a test and user scores for a test attempt.
    /// If TestAttemptId is null, the row is for the Test's max scores.
    /// If TestAttemptId is not null, the row is for a user's attempt scores.
    /// </summary>
    public class TestScoreSummary
    {
        public Guid TestScoreSummaryId { get; set; }
        public Guid TestId { get; set; } // Always required
        public Guid? TestAttemptId { get; set; } // Null for max score, not null for user attempt

        public int kanji_score { get; set; } = 0;
        public int kanji_max_score { get; set; } = 0;
        public int vocab_score { get; set; } = 0;
        public int vocab_max_score { get; set; } = 0;
        public int grammar_score { get; set; } = 0;
        public int grammar_max_score { get; set; } = 0;
        public int reading_score { get; set; } = 0;
        public int reading_max_score { get; set; } = 0;
        public int listening_score { get; set; } = 0;
        public int listening_max_score { get; set; } = 0;
        public int total_score { get; set; } = 0;
        public int total_max_score { get; set; } = 0;
        

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
        public virtual TestAttempt? TestAttempt { get; set; }
    }
}