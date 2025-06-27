namespace JCertPreApplication.Domain.Configuration
{
    public class JwtConfiguration
    {
        public const string SectionName = "Jwt";
        
        public string SecretKey { get; set; } = string.Empty;
        public string RefreshSecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; } = 60;

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
                throw new InvalidOperationException("JWT SecretKey is required. Please configure JWT_SECRET_KEY in your .env file.");

            if (string.IsNullOrWhiteSpace(RefreshSecretKey))
                throw new InvalidOperationException("JWT RefreshSecretKey is required. Please configure JWT_REFRESH_SECRET_KEY in your .env file.");

            if (string.IsNullOrWhiteSpace(Issuer))
                throw new InvalidOperationException("JWT Issuer is required. Please configure JWT_ISSUER in your .env file.");

            if (string.IsNullOrWhiteSpace(Audience))
                throw new InvalidOperationException("JWT Audience is required. Please configure JWT_AUDIENCE in your .env file.");

            // Validate secret key strength
            if (SecretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long for security reasons.");

            if (RefreshSecretKey.Length < 32)
                throw new InvalidOperationException("JWT RefreshSecretKey must be at least 32 characters long for security reasons.");
        }
    }
} 