using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing TestTemplateType CRUD operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestTemplateTypeController : ControllerBase
{
    private readonly ITestTemplateTypeService _service;

    public TestTemplateTypeController(ITestTemplateTypeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all test template types with search, filter, and paging.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] CourseLevel? level, [FromQuery] TestType? type, [FromQuery] bool? isActive, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(search, level, type, isActive, pageIndex, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Create a new test template type.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestTemplateTypeDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { }, result);
    }

    /// <summary>
    /// Update a test template type by id.
    /// </summary>
    [HttpPut("{testTemplateTypeId:guid}")]
    public async Task<IActionResult> Update(Guid testTemplateTypeId, [FromBody] UpdateTestTemplateTypeDto dto)
    {
        var result = await _service.UpdateAsync(testTemplateTypeId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Delete a test template type by id.
    /// </summary>
    [HttpDelete("{testTemplateTypeId:guid}")]
    public async Task<IActionResult> Delete(Guid testTemplateTypeId)
    {
        await _service.DeleteAsync(testTemplateTypeId);
        return NoContent();
    }

    /// <summary>
    /// Update only the isActive field of a test template type by id.
    /// </summary>
    [HttpPatch("{testTemplateTypeId:guid}/is-active")]
    public async Task<IActionResult> UpdateIsActive(Guid testTemplateTypeId, [FromQuery] bool isActive)
    {
        var result = await _service.UpdateIsActiveAsync(testTemplateTypeId, isActive);
        return Ok(result);
    }
}