namespace JCertPreApplication.API.Configuration
{
    public class JwtConfiguration
    {
        public string SecretKey { get; set; } = string.Empty;
        public string RefreshSecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; } = 60;

        public static JwtConfiguration LoadFromEnvironment()
        {
            var jwtConfig = new JwtConfiguration();

            // Load from environment variables only (from .env file)
            jwtConfig.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
                                ?? throw new InvalidOperationException("JWT_SECRET_KEY environment variable not found. Please set it in your .env file.");

            jwtConfig.RefreshSecretKey = Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET_KEY") 
                                       ?? throw new InvalidOperationException("JWT_REFRESH_SECRET_KEY environment variable not found. Please set it in your .env file.");

            jwtConfig.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
                             ?? throw new InvalidOperationException("JWT_ISSUER environment variable not found. Please set it in your .env file.");

            jwtConfig.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
                               ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable not found. Please set it in your .env file.");

            if (int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES"), out int expiry))
            {
                jwtConfig.ExpiryInMinutes = expiry;
            }

            // Validate secret key strength
            if (jwtConfig.SecretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT_SECRET_KEY must be at least 32 characters long for security reasons.");
            }

            if (jwtConfig.RefreshSecretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT_REFRESH_SECRET_KEY must be at least 32 characters long for security reasons.");
            }

            return jwtConfig;
        }
    }
} 