namespace JCertPreApplication.Domain.Configuration
{
    public class GeminiConfiguration
    {
        public const string SectionName = "GeminiAI";
        
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/v1beta";
        public string Model { get; set; } = "gemini-2.5-flash";
        public int RateLimitRPM { get; set; } = 10;
        public int TimeoutSeconds { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new InvalidOperationException("Gemini API Key is required. Please configure GeminiAI__ApiKey in your .env file.");

            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new InvalidOperationException("Gemini Base URL is required. Please configure GeminiAI__BaseUrl in your .env file.");

            if (string.IsNullOrWhiteSpace(Model))
                throw new InvalidOperationException("Gemini Model is required. Please configure GeminiAI__Model in your .env file.");

            if (RateLimitRPM <= 0)
                throw new InvalidOperationException("Gemini Rate Limit RPM must be greater than 0. Please configure GeminiAI__RateLimitRPM in your .env file.");

            if (TimeoutSeconds <= 0)
                throw new InvalidOperationException("Gemini Timeout must be greater than 0. Please configure GeminiAI__TimeoutSeconds in your .env file.");

            if (MaxRetries < 0)
                throw new InvalidOperationException("Gemini Max Retries must be 0 or greater. Please configure GeminiAI__MaxRetries in your .env file.");
        }
    }
}
