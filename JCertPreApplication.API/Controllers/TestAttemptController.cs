using JCertPreApplication.Application.Dtos.TestAttempt;
using JCertPreApplication.Application.Features.TestAttempts;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages test attempts.
    /// </summary>
    [ApiController]
    [Route("api/test-attempts")]
    [Tags("TestAttempts")]
    [Produces("application/json")]
    [Authorize]
    public class TestAttemptController : ControllerBase
{
    private readonly ITestAttemptService _service;

    public TestAttemptController(
        ITestAttemptService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        
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
           
            throw;
        }
    }

    /// <summary>
    /// Get a test attempt by Id with score summary.
    /// </summary>
    [HttpGet("{attemptId}/with-score-summary")]
    public async Task<IActionResult> GetAttemptWithScoreSummary(Guid attemptId)
    {
        var (attempt, scoreSummary) = await _service.GetAttemptWithScoreSummaryAsync(attemptId);
        if (attempt == null)
            return NotFound();
        return Ok(new { Attempt = attempt, ScoreSummary = scoreSummary });
    }
}}
