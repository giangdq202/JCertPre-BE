using JCertPreApplication.API.Common;
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

        /// <summary>
        /// Clear all cached data
        /// </summary>
        /// <remarks>
        /// Removes all cached data from the Redis cache system.
        /// 
        /// Warning: This operation is irreversible!
        /// 
        /// What Gets Cleared:
        /// - All application cache entries
        /// - Session data (users will need to re-authenticate)
        /// - Temporary data and performance caches
        /// - Any custom cached objects
        /// 
        /// Use Cases:
        /// - Development environment cleanup
        /// - Cache corruption recovery
        /// - System maintenance and updates
        /// - Testing cache-dependent functionality
        /// 
        /// Performance Impact:
        /// - Immediate: Cache warming will be required
        /// - Short-term: Increased database load as cache rebuilds
        /// - Users may experience slower response times temporarily
        /// 
        /// Best Practices:
        /// - Use during low-traffic periods
        /// - Notify users of potential temporary slowdowns
        /// - Consider selective cache clearing instead of full flush
        /// - Monitor system performance after cache clear
        /// 
        /// Production Considerations:
        /// - Should require administrative privileges
        /// - Consider implementing confirmation mechanisms
        /// - Log cache clear operations for auditing
        /// - Have cache warming strategies ready
        /// </remarks>
        /// <returns>Confirmation message indicating cache has been cleared</returns>
        /// <response code="200">Cache cleared successfully. All cached data has been removed.</response>
        /// <response code="500">Internal server error during cache clearing operation.</response>
        [HttpDelete("clear")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize(Roles = "Admin")] // Uncomment when authentication is implemented
        public async Task<IActionResult> ClearCache()
        {
            await _cacheService.ClearAllAsync();
            return Ok(new { message = "Cache cleared successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _cacheService.GetDataAsync(id);
            return Ok(result);
        }
    }
}

