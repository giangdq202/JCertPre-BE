using JCertPreApplication.Application.Features.TestAttempts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestAttemptController : ControllerBase
{
    private readonly ITestAttemptService _service;
    private readonly ILogger<TestAttemptController> _logger;

    public TestAttemptController(
        ITestAttemptService service,
        ILogger<TestAttemptController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Start a test attempt.
    /// </summary>
    [HttpPost("start")]
    public async Task<IActionResult> StartTestAttempt([FromBody] StartTestAttemptDto dto)
    {
        try
        {
            // The service handles both test creation AND monitoring activation
            var result = await _service.StartTestAttemptAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start test attempt for user {UserId}, test {TestId}", dto.UserId, dto.TestId);
            throw;
        }
    }

    /// <summary>
    /// Submit a test attempt.
    /// </summary>
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitTestAttempt([FromBody] SubmitTestAttemptDto dto)
    {
        try
        {
            var result = await _service.SubmitTestAttemptAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit test attempt {AttemptId}", dto.AttemptId);
            throw;
        }
    }

    /// <summary>
    /// Get all test attempts by user id.
    /// </summary>
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetAllByUserId(Guid userId)
    {
        try
        {
            var result = await _service.GetAllByUserIdAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get test attempts for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Update the status of a test attempt.
    /// </summary>
    [HttpPut("update-status/{attemptId}")]
    public async Task<IActionResult> UpdateStatus(Guid attemptId, [FromBody] TestAttemptStatus status)
    {
        try
        {
            var result = await _service.UpdateStatusAsync(attemptId, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update status for test attempt {AttemptId}", attemptId);
            throw;
        }
    }
}