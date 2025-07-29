namespace JCertPreApplication.Domain.Configuration
{
    public class CloudinaryConfiguration
    {
        public const string SectionName = "Cloudinary";

        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public bool Secure { get; set; } = true;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(CloudName))
                throw new InvalidOperationException("Cloudinary CloudName is required. Please configure Cloudinary__CloudName in your .env file.");

            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new InvalidOperationException("Cloudinary ApiKey is required. Please configure Cloudinary__ApiKey in your .env file.");

            if (string.IsNullOrWhiteSpace(ApiSecret))
                throw new InvalidOperationException("Cloudinary ApiSecret is required. Please configure Cloudinary__ApiSecret in your .env file.");
        }
    }
} 