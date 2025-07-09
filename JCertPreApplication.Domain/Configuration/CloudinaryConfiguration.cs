namespace JCertPreApplication.Domain.Configuration
{
    public class CloudinaryConfiguration
    {
        public const string SectionName = "Cloudinary";

        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public bool Secure { get; set; } = true;
    }
} 