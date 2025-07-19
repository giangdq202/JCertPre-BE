using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Dtos.QuestionAttachment;
using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// API endpoints for managing Question entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// Get all questions.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var questions = await _questionService.GetAllAsync();
        var dtos = questions.Select(MapToQuestionDto);
        return Ok(dtos);
    }

    /// <summary>
    /// Get a question by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question == null)
            return NotFound();
        return Ok(MapToQuestionDto(question));
    }

    /// <summary>
    /// Create a new question.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _questionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.questionId }, MapToQuestionDto(created));
    }

    /// <summary>
    /// Update an existing question.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _questionService.UpdateAsync(id, dto);
        return Ok(MapToQuestionDto(updated));
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
        var result = await _questionService.GetPaginatedWithDetailsAsync(search, pageIndex, pageSize, contentName, level, subContentName);

        var dto = new Pagination<QuestionDto>
        {
            PageIndex = result.PageIndex,
            PageSize = result.PageSize,
            TotalItemsCount = result.TotalItemsCount,
            Items = result.Items.Select(MapToQuestionDto).ToList()
        };
        return Ok(dto);
    }

    // Mapping logic at controller layer
    private static QuestionDto MapToQuestionDto(Question question)
    {
        var subContent = question.SubContent;
        return new QuestionDto
        {
            Id = question.questionId,
            Content = question.questionText,
            Explanation = question.explanation,
            Points = question.points,
            Difficulty = question.difficulty, // Add this line
            Choices = question.Choices?.Select(c => new ChoiceReadDto
            {
                Id = c.choiceId,
                Content = c.choiceText,
                IsCorrect = c.isCorrect,
                QuestionId = c.questionId
            }).ToList(),
            QuestionAttachments = question.QuestionAttachments?.Select(a => new QuestionAttachmentDto
            {
                MediaUrl = a.mediaUrl,
                MediaType = a.mediaType
            }).ToList(),
            ContentName = subContent?.ContentName.ToString() ?? "",
            ContentNameDescription = subContent != null ? EnumHelper.GetEnumDescription(subContent.ContentName) : "",
            Level = subContent?.Level.ToString() ?? "",
            LevelDescription = subContent != null ? EnumHelper.GetEnumDescription(subContent.Level) : "",
            SubContentName = subContent?.SubContentName.ToString() ?? "",
            SubContentNameDescription = subContent != null ? EnumHelper.GetEnumDescription(subContent.SubContentName) : ""
        };
    }
}