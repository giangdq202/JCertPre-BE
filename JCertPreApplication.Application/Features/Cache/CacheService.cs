using JCertPreApplication.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Cache
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task<string> GetDataAsync(string id)
        {
            var cacheKey = $"data:{id}";
            var cachedData = await _cacheRepository.GetAsync<string>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }

            var data = $"Fetched data for ID {id}";
            await _cacheRepository.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            return data;
        }
    }
}
