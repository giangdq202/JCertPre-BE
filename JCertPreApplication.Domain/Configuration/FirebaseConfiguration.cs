namespace JCertPreApplication.Domain.Configuration
{
    public class FirebaseConfiguration
    {
        public const string SectionName = "Firebase";

        public string Type { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string PrivateKeyId { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string AuthUri { get; set; } = string.Empty;
        public string TokenUri { get; set; } = string.Empty;
        public string AuthProviderX509CertUrl { get; set; } = string.Empty;
        public string ClientX509CertUrl { get; set; } = string.Empty;
        public string UniverseDomain { get; set; } = string.Empty;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Type))
                throw new InvalidOperationException("Firebase Type is required. Please configure Firebase__Type in your .env file.");

            if (string.IsNullOrWhiteSpace(ProjectId))
                throw new InvalidOperationException("Firebase ProjectId is required. Please configure Firebase__ProjectId in your .env file.");

            if (string.IsNullOrWhiteSpace(PrivateKeyId))
                throw new InvalidOperationException("Firebase PrivateKeyId is required. Please configure Firebase__PrivateKeyId in your .env file.");

            if (string.IsNullOrWhiteSpace(PrivateKey))
                throw new InvalidOperationException("Firebase PrivateKey is required. Please configure Firebase__PrivateKey in your .env file.");

            if (string.IsNullOrWhiteSpace(ClientEmail))
                throw new InvalidOperationException("Firebase ClientEmail is required. Please configure Firebase__ClientEmail in your .env file.");

            if (string.IsNullOrWhiteSpace(ClientId))
                throw new InvalidOperationException("Firebase ClientId is required. Please configure Firebase__ClientId in your .env file.");
        }
    }
} 