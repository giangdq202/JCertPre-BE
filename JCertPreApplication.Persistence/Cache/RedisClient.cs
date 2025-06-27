using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace JCertPreApplication.Persistence.Cache
{
    public class RedisClient
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        public RedisClient(IOptions<RedisConfiguration> redisOptions)
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var connectionString = redisOptions.Value.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Redis connection string is not configured.");
                
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }

        public ConnectionMultiplexer Connection => _lazyConnection.Value;
    }
}
