using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages test templates.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Tags("TestTemplates")]
    [Produces("application/json")]
    public class TestTemplateController : ControllerBase
{
    private readonly ITestTemplateService _service;

    public TestTemplateController(ITestTemplateService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all test templates by testTemplateTypeId.
    /// </summary>
    [HttpGet("by-type/{testTemplateTypeId:guid}")]
    public async Task<IActionResult> GetAllByTypeId(Guid testTemplateTypeId)
    {
        var result = await _service.GetAllByTypeIdAsync(testTemplateTypeId);
        return Ok(result);
    }

    /// <summary>
    /// Create a new test template.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestTemplateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAllByTypeId), new { testTemplateTypeId = result.TestTemplateTypeId }, result);
    }

    /// <summary>
    /// Update a test template by templateId.
    /// </summary>
    [HttpPut("{templateId:guid}")]
    public async Task<IActionResult> Update(Guid templateId, [FromBody] UpdateTestTemplateDto dto)
    {
        var result = await _service.UpdateAsync(templateId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Delete a test template by templateId.
    /// </summary>
    [HttpDelete("{templateId:guid}")]
    public async Task<IActionResult> Delete(Guid templateId)
    {
        await _service.DeleteAsync(templateId);
        return NoContent();
    }
}}
