using System;

namespace JCertPreApplication.Domain.Entities
{
    /// <summary>
    /// Stores pre-calculated max scores for a test and user scores for a test attempt.
    /// If TestAttemptId is null, the row is for the Test's max scores.
    /// If TestAttemptId is not null, the row is for a user's attempt scores.
    /// Each field is stored as "userScore/maxScore" (e.g., "18/20") for attempts, or just maxScore for tests.
    /// </summary>
    public class TestScoreSummary
    {
        public Guid TestScoreSummaryId { get; set; }
        public Guid TestId { get; set; } // Always required
        public Guid? TestAttemptId { get; set; } // Null for max score, not null for user attempt

        // 5 score fields as string (e.g., "18/20" or "20")
        public string? KanjiScore { get; set; }
        public string? VocabularyScore { get; set; }
        public string? GrammarScore { get; set; }
        public string? ReadingScore { get; set; }
        public string? ListeningScore { get; set; }
        public string? TotalScore { get; set; } 

        // Navigation properties
        public virtual Test Test { get; set; } = null!;
        public virtual TestAttempt? TestAttempt { get; set; }
    }
}