using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestTemplateController : ControllerBase
{
    private readonly ITestTemplateService _service;

    public TestTemplateController(ITestTemplateService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all test templates with search, filter, and paging.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] CourseLevel? level, [FromQuery] TestType? type, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(search, level, type, pageIndex, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get a test template by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Create a new test template.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestTemplateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.TemplateId }, result);
    }

    /// <summary>
    /// Update an existing test template.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestTemplateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Update only the isActive field of a test template.
    /// </summary>
    [HttpPatch("{id:guid}/is-active")]
    public async Task<IActionResult> UpdateIsActive(Guid id, [FromQuery] bool isActive)
    {
        var result = await _service.UpdateIsActiveAsync(id, isActive);
        return Ok(result);
    }

    /// <summary>
    /// Delete a test template.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}