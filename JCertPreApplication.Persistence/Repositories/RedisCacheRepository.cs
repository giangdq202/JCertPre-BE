using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Persistence.Cache;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly StackExchange.Redis.IDatabase _database; // Updated type to match StackExchange.Redis.IDatabase

        public RedisCacheRepository()
        {
            _database = RedisClient.Connection.GetDatabase(); // Correct type usage for StackExchange.Redis.IDatabase
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key); // StringGetAsync is part of StackExchange.Redis.IDatabase
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serialized = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serialized, expiry); // StringSetAsync is part of StackExchange.Redis.IDatabase
        }

        public async Task DeleteAsync(string key)
        {
            await _database.KeyDeleteAsync(key); // KeyDeleteAsync is part of StackExchange.Redis.IDatabase
        }
    }
}
