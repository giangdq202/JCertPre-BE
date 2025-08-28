using JCertPreApplication.Application.Dtos.SubContent;
using JCertPreApplication.Application.Features.SubContents;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages sub-content operations.
    /// </summary>
    [Route("api/subcontents")]
    [ApiController]
    [Tags("SubContents")]
    [Produces("application/json")]
    [Authorize]
    public class SubContentsController : ControllerBase
{
    private readonly ISubContentService _service;

    public SubContentsController(ISubContentService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Get all SubContents with search, paging, and filtering by enums.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(Pagination<SubContentDto>), StatusCodes.Status200OK)]
    [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] CourseLevel? level,
        [FromQuery] ContentName? contentName,
        [FromQuery] SubContentName? subContentName,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllAsync(search, level, contentName, subContentName, pageIndex, pageSize);

        // Map entities to DTOs at controller layer, including enum descriptions
        var dto = new Pagination<SubContentDto>
        {
            PageIndex = result.PageIndex,
            PageSize = result.PageSize,
            TotalItemsCount = result.TotalItemsCount,
            Items = result.Items.Select(x => new SubContentDto
            {
                SubContentId = x.SubContentId,
                SubContentName = x.SubContentName.ToString(),
                SubContentNameDescription = EnumHelper.GetEnumDescription(x.SubContentName),
                Level = x.Level.ToString(),
                LevelDescription = EnumHelper.GetEnumDescription(x.Level),
                ContentName = x.ContentName.ToString(),
                ContentNameDescription = EnumHelper.GetEnumDescription(x.ContentName)
            }).ToList()
        };
        return Ok(dto);
    }

    /// <summary>
    /// Create a new SubContent.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SubContentDto), StatusCodes.Status201Created)]
    [Authorize(Roles = "ACADEMIC_MANAGER")]
    public async Task<IActionResult> Create([FromBody] CreateSubContentDto dto)
    {
        var entity = await _service.CreateAsync(dto);
        var result = new SubContentDto
        {
            SubContentId = entity.SubContentId,
            SubContentName = entity.SubContentName.ToString(),
            SubContentNameDescription = EnumHelper.GetEnumDescription(entity.SubContentName),
            Level = entity.Level.ToString(),
            LevelDescription = EnumHelper.GetEnumDescription(entity.Level),
            ContentName = entity.ContentName.ToString(),
            ContentNameDescription = EnumHelper.GetEnumDescription(entity.ContentName)
        };
        return CreatedAtAction(nameof(GetAll), new { subContentId = result.SubContentId }, result);
    }

    /// <summary>
    /// Update an existing SubContent by ID.
    /// </summary>
    [HttpPut("{subContentId}")]
    [ProducesResponseType(typeof(SubContentDto), StatusCodes.Status200OK)]
    [Authorize(Roles = "ACADEMIC_MANAGER")]
    public async Task<IActionResult> Update(Guid subContentId, [FromBody] UpdateSubContentDto dto)
    {
        var entity = await _service.UpdateAsync(subContentId, dto);
        var result = new SubContentDto
        {
            SubContentId = entity.SubContentId,
            SubContentName = entity.SubContentName.ToString(),
            SubContentNameDescription = EnumHelper.GetEnumDescription(entity.SubContentName),
            Level = entity.Level.ToString(),
            LevelDescription = EnumHelper.GetEnumDescription(entity.Level),
            ContentName = entity.ContentName.ToString(),
            ContentNameDescription = EnumHelper.GetEnumDescription(entity.ContentName)
        };
        return Ok(result);
    }

    /// <summary>
    /// Delete a SubContent by ID.
    /// </summary>
    [HttpDelete("{subContentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Roles = "ACADEMIC_MANAGER")]
    public async Task<IActionResult> Delete(Guid subContentId)
    {
        await _service.DeleteAsync(subContentId);
        return NoContent();
    }

    /// <summary>
    /// Get all values and descriptions for SubContentName enum.
    /// </summary>
    [HttpGet("enum-values/subcontent-name")]
    [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
    public async Task<IActionResult> GetSubContentNameEnumValues()
    {
        var values = await _service.GetSubContentNameEnumValuesAsync();
        return Ok(values);
    }

    /// <summary>
    /// Get all values and descriptions for CourseLevel enum.
    /// </summary>
    [HttpGet("enum-values/level")]
    [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
    public async Task<IActionResult> GetLevelEnumValues()
    {
        var values = await _service.GetLevelEnumValuesAsync();
        return Ok(values);
    }

    /// <summary>
    /// Get all values and descriptions for ContentName enum.
    /// </summary>
    [HttpGet("enum-values/content-name")]
    [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
    public async Task<IActionResult> GetContentNameEnumValues()
    {
        var values = await _service.GetContentNameEnumValuesAsync();
        return Ok(values);
    }
}}
