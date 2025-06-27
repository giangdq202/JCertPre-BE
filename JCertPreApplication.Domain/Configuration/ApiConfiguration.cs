namespace JCertPreApplication.Domain.Configuration
{
    public class ApiConfiguration
    {
        public const string SectionName = "Api";
        
        public string Environment { get; set; } = "Development";
        public string Urls { get; set; } = "https://localhost:7001;http://localhost:5001";
        public bool ShowConfigurationStatus { get; set; } = true;
    }
} 