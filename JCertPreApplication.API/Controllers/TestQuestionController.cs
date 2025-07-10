using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Utilities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test-questions")]
public class TestQuestionController : ControllerBase
{
    private readonly ITestQuestionService _service;

    public TestQuestionController(ITestQuestionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Add a question to a test.
    /// </summary>
    [HttpPost("{testId}/add/{questionId}")]
    public async Task<IActionResult> AddQuestionToTest(Guid testId, Guid questionId)
    {
        await _service.AddQuestionToTestAsync(testId, questionId);
        return NoContent();
    }

    /// <summary>
    /// Get all questions from a test (paginated).
    /// </summary>
    [HttpGet("{testId}/questions")]
    public async Task<IActionResult> GetQuestionsByTestId(Guid testId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetQuestionsByTestIdAsync(testId, pageIndex, pageSize);
        return Ok(MapToPaginationDto(result));
    }

    /// <summary>
    /// Get a specific question from a test.
    /// </summary>
    [HttpGet("{testId}/question/{questionId}")]
    public async Task<IActionResult> GetTestQuestion(Guid testId, Guid questionId)
    {
        var result = await _service.GetTestQuestionAsync(testId, questionId);
        if (result == null)
            return NotFound();
        return Ok(MapToTestQuestionDto(result));
    }

    /// <summary>
    /// Get all question IDs from a test.
    /// </summary>
    [HttpGet("{testId}/question-ids")]
    public async Task<IActionResult> GetAllQuestionIdsByTestId(Guid testId)
    {
        var ids = await _service.GetAllQuestionIdsByTestIdAsync(testId);
        return Ok(ids);
    }

    /// <summary>
    /// Update isActive field for a test question.
    /// </summary>
    [HttpPatch("{testId}/question/{questionId}/is-active")]
    public async Task<IActionResult> UpdateIsActive(Guid testId, Guid questionId, [FromBody] bool isActive)
    {
        await _service.UpdateIsActiveAsync(testId, questionId, isActive);
        return NoContent();
    }

    /// <summary>
    /// Delete a question from a test.
    /// </summary>
    [HttpDelete("{testId}/question/{questionId}")]
    public async Task<IActionResult> DeleteTestQuestion(Guid testId, Guid questionId)
    {
        await _service.DeleteTestQuestionAsync(testId, questionId);
        return NoContent();
    }

    private static TestQuestionDto MapToTestQuestionDto(TestQuestion tq)
    {
        return new TestQuestionDto
        {
            TestQuestionId = tq.testQuestionId,
            TestId = tq.testId,
            QuestionId = tq.questionId,
            IsActive = tq.isActive,
            Question = tq.Question == null ? null : new QuestionInTestDto
            {
                Id = tq.Question.questionId,
                Content = tq.Question.questionText,
                Explanation = tq.Question.explanation,
                Points = tq.Question.points,
                QuestionType = tq.Question.questionType,
                Choices = tq.Question.Choices?.Select(c => new ChoiceReadDto
                {
                    Id = c.choiceId,
                    Content = c.choiceText,
                    IsCorrect = c.isCorrect,
                    QuestionId = c.questionId
                }).ToList()
            }
        };
    }

    private static Pagination<TestQuestionDto> MapToPaginationDto(Pagination<TestQuestion> paged)
    {
        return new Pagination<TestQuestionDto>
        {
            PageIndex = paged.PageIndex,
            PageSize = paged.PageSize,
            TotalItemsCount = paged.TotalItemsCount,
            Items = paged.Items.Select(MapToTestQuestionDto).ToList()
        };
    }
}