namespace JCertPreApplication.Domain.Configuration
{
    public class ApiConfiguration
    {
        public const string SectionName = "Api";
        
        public string Environment { get; set; } = "Development";
        public string Urls { get; set; } = "http://localhost:5001";
        public string PublicUrl { get; set; } = "http://localhost:5001";
        public bool ShowConfigurationStatus { get; set; } = true;
    }
} 