using JCertPreApplication.Application.Features.Cache;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
        [Route("api/[controller]")]
        public class CacheController : ControllerBase
        {
            private readonly ICacheService _service;

            public CacheController(ICacheService service)
            {
                _service = service;
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> Get(string id)
            {
                var result = await _service.GetDataAsync(id);
                return Ok(result);
            }
        }
    }

