using JCertPreApplication.Application.Features.Cache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages cache operations.
    /// </summary>
    [Route("api/cache")]
    [ApiController]
    [Tags("Cache")]
    [Produces("application/json")]
    [Authorize]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        /// <summary>
        /// Retrieves cached data by ID.
        /// </summary>
        /// <param name="id">Cache key identifier.</param>
        /// <returns>The cached data if found.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _cacheService.GetDataAsync(id);
            return Ok(result);
        }
    }
}

