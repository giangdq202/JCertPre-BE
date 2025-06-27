namespace JCertPreApplication.API.Configuration
{
    public class CorsConfiguration
    {
        public const string SectionName = "Cors";
        
        public string AllowedOrigins { get; set; } = string.Empty;
        
        public string[] GetAllowedOriginsArray()
        {
            if (string.IsNullOrWhiteSpace(AllowedOrigins))
                return new[] { "http://localhost:3000", "https://localhost:3000" }; // fallback origins
            
            return AllowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(origin => origin.Trim())
                                .ToArray();
        }
    }
} 