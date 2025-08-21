using JCertPreApplication.Application.Dtos.TestTemplateConfig;
using JCertPreApplication.Application.Features.TestTemplateConfigs;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Controller for managing TestTemplateConfig CRUD operations.
    /// </summary>
    [ApiController]
    [Route("api/test-template-configs")]
    [Tags("TestTemplateConfigs")]
    [Produces("application/json")]
    public class TestTemplateConfigController : ControllerBase
    {
        private readonly ITestTemplateConfigService _service;

        public TestTemplateConfigController(ITestTemplateConfigService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all test template configs by templateId.
        /// </summary>
        [HttpGet("by-template/{templateId:guid}")]
        public async Task<IActionResult> GetAllByTemplateId(Guid templateId)
        {
            var result = await _service.GetAllByTemplateIdAsync(templateId);
            return Ok(result);
        }

        /// <summary>
        /// Get a test template config by configId.
        /// </summary>
        [HttpGet("{configId:guid}")]
        public async Task<IActionResult> GetByConfigId(Guid configId)
        {
            var result = await _service.GetByConfigIdAsync(configId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Create a test template config by templateId.
        /// </summary>
        [HttpPost("{templateId:guid}")]
        public async Task<IActionResult> Create(Guid templateId, [FromBody] CreateTestTemplateConfigDto dto)
        {
            var result = await _service.CreateAsync(templateId, dto);
            return CreatedAtAction(nameof(GetByConfigId), new { configId = result.configId }, result);
        }

        /// <summary>
        /// Update a test template config by configId.
        /// </summary>
        [HttpPut("{configId:guid}")]
        public async Task<IActionResult> Update(Guid configId, [FromBody] UpdateTestTemplateConfigDto dto)
        {
            var result = await _service.UpdateAsync(configId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Delete a test template config by configId.
        /// </summary>
        [HttpDelete("{configId:guid}")]
        public async Task<IActionResult> Delete(Guid configId)
        {
            await _service.DeleteAsync(configId);
            return NoContent();
        }
    }
}