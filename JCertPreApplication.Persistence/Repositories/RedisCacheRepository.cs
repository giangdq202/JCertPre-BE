using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Persistence.Cache;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace JCertPreApplication.Persistence.Repositories
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly StackExchange.Redis.IDatabase _database;
        private readonly ILogger<RedisCacheRepository> _logger;

        public RedisCacheRepository(RedisClient redisClient, ILogger<RedisCacheRepository> logger)
        {
            _database = redisClient.Connection.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_CACHE_KEY", "Cache key cannot be null or empty.");

                var value = await _database.StringGetAsync(key);
                return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize cached value for key: {Key}", key);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_DESERIALIZATION_ERROR", 
                    "Failed to deserialize cached data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get value from cache for key: {Key}", key);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_GET_ERROR", 
                    "Failed to retrieve data from cache.");
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_CACHE_KEY", "Cache key cannot be null or empty.");

                if (value == null)
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_CACHE_VALUE", "Cache value cannot be null.");

                var serialized = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, serialized, expiry);
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to serialize value for cache key: {Key}", key);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_SERIALIZATION_ERROR", 
                    "Failed to serialize data for caching.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set value in cache for key: {Key}", key);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_SET_ERROR", 
                    "Failed to store data in cache.");
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ApiException(HttpStatusCode.BadRequest, "INVALID_CACHE_KEY", "Cache key cannot be null or empty.");

                await _database.KeyDeleteAsync(key);
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete value from cache for key: {Key}", key);
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_DELETE_ERROR", 
                    "Failed to delete data from cache.");
            }
        }

        public async Task ClearAllAsync()
        {
            try
            {
                var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());
                await server.FlushDatabaseAsync(_database.Database);
                _logger.LogInformation("Cache cleared successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all cache data");
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_CLEAR_ERROR", 
                    "Failed to clear all cache data.");
            }
        }
    }
}
