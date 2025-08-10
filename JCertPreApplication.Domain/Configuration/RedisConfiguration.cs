namespace JCertPreApplication.Domain.Configuration
{
    public class RedisConfiguration
    {
        public const string SectionName = "Redis";
        
        public string ConnectionString { get; set; } = string.Empty;
        
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new InvalidOperationException("Redis ConnectionString is required. Please configure Redis__ConnectionString in your .env file.");
        }
    }
} 