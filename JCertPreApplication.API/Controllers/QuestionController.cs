using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Dtos.QuestionAttachment;
using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages question entities.
    /// </summary>
    [ApiController]
    [Route("api/questions")]
    [Tags("Questions")]
    [Produces("application/json")]
    public class QuestionController : ControllerBase
    {
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    /// <summary>
    /// Get a question by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _questionService.GetByIdAsync(id);
        if (dto == null)
            return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Create a new question.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateQuestionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _questionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing question.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateQuestionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _questionService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    /// <summary>
    /// Delete a question by its ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _questionService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get paginated questions with details (choices, attachments), filterable by subcontent fields.
    /// </summary>
    [HttpGet("paging-details")]
    public async Task<IActionResult> GetPagingWithDetails(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] ContentName? contentName = null,
        [FromQuery] CourseLevel? level = null,
        [FromQuery] SubContentName? subContentName = null)
    {
        var dto = await _questionService.GetPaginatedWithDetailsAsync(search, pageIndex, pageSize, contentName, level, subContentName);
        return Ok(dto);
    }

    /// <summary>
    /// Get a question by its ID for test by the ID.
    /// </summary>
    [HttpGet("test/{id:guid}")]
    public async Task<IActionResult> GetByIdForTest(Guid id)
    {
        var dto = await _questionService.GetByIdForTestAsync(id);
        if (dto == null)
            return NotFound();
        return Ok(dto);
    }

    /// <summary>
/// Get paginated active questions with details (choices, attachments), filterable by subcontent fields.
/// </summary>
[HttpGet("paging-details/active")]
public async Task<IActionResult> GetPagingActiveWithDetails(
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] ContentName? contentName = null,
    [FromQuery] CourseLevel? level = null,
    [FromQuery] SubContentName? subContentName = null,
    [FromQuery] QuestionDifficulty? difficulty = null)
{
    var dto = await _questionService.GetPaginatedActiveWithDetailsAsync(search, pageIndex, pageSize, contentName, level, subContentName, difficulty);
    return Ok(dto);
}

[HttpPost("import")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> ImportQuestions([FromForm] ImportQuestionsRequestDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _questionService.ImportQuestionsAsync(dto);

    if (result.FailedCount > 0 && !string.IsNullOrEmpty(result.FailedFileUrl))
    {
        var fileBytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(Path.GetTempPath(), result.FailedFileUrl));
        System.IO.File.Delete(Path.Combine(Path.GetTempPath(), result.FailedFileUrl));

        // Add summary info to response headers
        Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
        Response.Headers.Append("X-Success-Count", result.SuccessCount.ToString());
        Response.Headers.Append("X-Failed-Count", result.FailedCount.ToString());

        return File(fileBytes, "application/json", "import_failed.json");
    }

    // No failed questions, return summary as JSON
    return Ok(new
    {
        totalCount = result.TotalCount,
        successCount = result.SuccessCount,
        failedCount = result.FailedCount
    });
}
}
}
