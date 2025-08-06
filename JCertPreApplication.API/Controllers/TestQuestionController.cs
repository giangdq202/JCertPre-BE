using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages test questions.
    /// </summary>
    [ApiController]
    [Route("api/test-questions")]
    [Tags("TestQuestions")]
    [Produces("application/json")]
    public class TestQuestionController : ControllerBase
{
    private readonly ITestQuestionService _service;

    public TestQuestionController(ITestQuestionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Add questions to a test using custom manual input.
    /// </summary>
    /// <remarks>
    /// Example request:
    /// [
    ///   {
    ///     "testId": "11111111-1111-1111-1111-111111111111",
    ///     "questionId": "22222222-2222-2222-2222-222222222222"
    ///   },
    ///   {
    ///     "testId": "33333333-3333-3333-3333-333333333333",
    ///     "questionId": "44444444-4444-4444-4444-444444444444"
    ///   }
    /// ]
    /// </remarks>
    [HttpPost("custom-manual/add")]
    public async Task<IActionResult> AddQuestionsCustomManual([FromBody] List<AddTestQuestionManualDto> testQuestionPairs)
    {
        var pairs = testQuestionPairs.Select(x => (x.TestId, x.QuestionId)).ToList();
        await _service.AddQuestionsCustomManualAsync(pairs);
        return NoContent();
    }

    /// <summary>
    /// Get all questions from a test (no paging).
    /// </summary>
    [HttpGet("{testId}/questions")]
    public async Task<IActionResult> GetQuestionsByTestId(Guid testId)
    {
        var result = await _service.GetQuestionsByTestIdAsync(testId);
        return Ok(result);
    }

    /// <summary>
    /// Delete a question from a test.
    /// </summary>
    [HttpDelete("{testQuestionId}")]
    public async Task<IActionResult> DeleteTestQuestion(Guid testQuestionId)
    {
        await _service.DeleteTestQuestionAsync(testQuestionId);
        return NoContent();
    }
    /// <summary>
    /// Add JLPT auto-generated questions to a test.
    /// </summary>
    /// <param name="testId">The ID of the test to add questions to.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [HttpPost("jlpt-auto/{testId}")]
    public async Task<IActionResult> AddQuestionsJLPTAuto(Guid testId)
    {
        await _service.AddQuestionsJLPTAutoAsync(testId);
        return Ok(new { message = "JLPT Auto questions added successfully." });
    }
}}
