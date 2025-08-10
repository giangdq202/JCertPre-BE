namespace JCertPreApplication.Domain.Configuration
{
    public class ApiConfiguration
    {
        public const string SectionName = "Api";
        
        public string Environment { get; set; } = "Development";
        public string Urls { get; set; } = "http://localhost:5001";
        public string PublicUrl { get; set; } = "http://localhost:5001";
        public bool ShowConfigurationStatus { get; set; } = true;
        
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Environment))
                throw new InvalidOperationException("API Environment is required. Please configure Api__Environment in your .env file.");
                
            if (string.IsNullOrWhiteSpace(Urls))
                throw new InvalidOperationException("API Urls is required. Please configure Api__Urls in your .env file.");
                
            if (string.IsNullOrWhiteSpace(PublicUrl))
                throw new InvalidOperationException("API PublicUrl is required. Please configure Api__PublicUrl in your .env file.");
                
            if (!Uri.TryCreate(PublicUrl, UriKind.Absolute, out var uri))
                throw new InvalidOperationException("API PublicUrl must be a valid URL.");
        }
    }
} 