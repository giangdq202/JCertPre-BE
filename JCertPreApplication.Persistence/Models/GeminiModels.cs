namespace JCertPreApplication.Persistence.Models
{
    public class GeminiRequest
    {
        public GeminiContent[] contents { get; set; } = Array.Empty<GeminiContent>();
        public GeminiGenerationConfig generationConfig { get; set; } = new();
    }

    public class GeminiContent
    {
        public GeminiPart[] parts { get; set; } = Array.Empty<GeminiPart>();
    }

    public class GeminiPart
    {
        public string text { get; set; } = string.Empty;
    }

    public class GeminiGenerationConfig
    {
        public string response_mime_type { get; set; } = "application/json";
        public object response_schema { get; set; } = new();
    }

    public class GeminiResponse
    {
        public GeminiCandidate[]? candidates { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent? content { get; set; }
    }

    public class GeminiQuestionData
    {
        public string questionText { get; set; } = string.Empty;
        public string explanation { get; set; } = string.Empty;
        public GeminiChoiceData[] choices { get; set; } = Array.Empty<GeminiChoiceData>();
    }

    public class GeminiChoiceData
    {
        public string content { get; set; } = string.Empty;
        public bool isCorrect { get; set; }
    }
}
