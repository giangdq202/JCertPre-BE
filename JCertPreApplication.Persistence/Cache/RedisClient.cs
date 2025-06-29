using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace JCertPreApplication.Persistence.Cache
{
    public class RedisClient
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private readonly ILogger<RedisClient> _logger;

        public RedisClient(IOptions<RedisConfiguration> redisOptions, ILogger<RedisClient> logger)
        {
            _logger = logger;
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                try
                {
                    var connectionString = redisOptions.Value.ConnectionString;
                    if (string.IsNullOrEmpty(connectionString))
                        throw new InvalidOperationException("Redis connection string is not configured.");
                    
                    _logger.LogInformation("Connecting to Redis with connection string: {ConnectionString}", 
                        MaskConnectionString(connectionString));
                    
                    var connection = ConnectionMultiplexer.Connect(connectionString);
                    
                    _logger.LogInformation("Successfully connected to Redis");
                    return connection;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to connect to Redis");
                    throw new InvalidOperationException("Failed to establish Redis connection. Please check your Redis configuration.", ex);
                }
            });
        }

        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        private static string MaskConnectionString(string connectionString)
        {
            // Simple masking for logging purposes
            if (connectionString.Contains("password=", StringComparison.OrdinalIgnoreCase))
            {
                return connectionString.Split(',')[0] + ",password=***";
            }
            return connectionString;
        }
    }
}
