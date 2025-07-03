using JCertPreApplication.Application.Features.Cache;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Cache Management API Controller
    /// </summary>
    /// <remarks>
    /// Provides cache management functionality for performance optimization and debugging.
    /// 
    /// Cache System:
    /// - Redis-based distributed caching
    /// - Key-value storage with TTL (Time To Live)
    /// - High-performance data retrieval
    /// - Session and application data caching
    /// 
    /// Use Cases:
    /// - Performance monitoring and debugging
    /// - Cache invalidation for data updates
    /// - Administrative cache management
    /// - Development and testing utilities
    /// 
    /// Security Note:
    /// These endpoints should be protected with appropriate authorization
    /// in production environments to prevent unauthorized cache manipulation.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Cache")]
    [Produces("application/json")]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _cacheService.GetDataAsync(id);
            return Ok(result);
        }
    }
}

