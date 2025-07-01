using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using System.Net;

namespace JCertPreApplication.Application.Features.Cache
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository ?? throw new ArgumentNullException(nameof(cacheRepository));
        }

        public async Task<string> GetDataAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ApiException(HttpStatusCode.BadRequest, "INVALID_ID", "ID cannot be null or empty.");

            try
            {
                var cacheKey = $"data:{id}";
                var cachedData = await _cacheRepository.GetAsync<string>(cacheKey);
                if (cachedData != null)
                {
                    return cachedData;
                }

                // Simulate fetching data from a data source
                var data = $"Fetched data for ID {id}";
                await _cacheRepository.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
                return data;
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (Exception)
            {
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_SERVICE_ERROR", 
                    "An error occurred while processing cache operations.");
            }
        }

        public async Task ClearAllAsync()
        {
            try
            {
                await _cacheRepository.ClearAllAsync();
            }
            catch (ApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (Exception)
            {
                throw new ApiException(HttpStatusCode.InternalServerError, "CACHE_CLEAR_ERROR", 
                    "An error occurred while clearing cache.");
            }
        }
    }
}
