namespace JCertPreApplication.Application.Contracts
{
    /// <summary>
    /// Interface for AI integration services to generate JLPT questions
    /// </summary>
    public interface IAIIntegration
    {
        /// <summary>
        /// Generates a JLPT question using AI based on the specified parameters
        /// </summary>
        /// <param name="level">JLPT level as string (N5, N4, N3, N2, N1)</param>
        /// <param name="contentName">Content type as string (Kanji, Vocabulary, Grammar, Reading)</param>
        /// <returns>Generated question data with choices</returns>
        Task<AIGeneratedQuestionResult> GenerateQuestionAsync(string level, string contentName);
    }

    /// <summary>
    /// Result model for AI-generated questions
    /// </summary>
    public class AIGeneratedQuestionResult
    {
        public string QuestionText { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public List<AIGeneratedChoice> Choices { get; set; } = new();
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Choice model for AI-generated questions
    /// </summary>
    public class AIGeneratedChoice
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
