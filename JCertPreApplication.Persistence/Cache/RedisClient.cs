using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Cache
{
    public class RedisClient
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection;

        static RedisClient()
        {
            LazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
                var connectionString = configuration.GetSection("Redis:ConnectionString").Value;
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }

        public static ConnectionMultiplexer Connection => LazyConnection.Value;
    }
}
