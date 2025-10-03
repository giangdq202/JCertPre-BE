using JCertPreApplication.Application.Dtos.TestAttempt;
using JCertPreApplication.Application.Features.TestAttempts;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> StartTestAttempt([FromBody] StartTestAttemptDto dto)
    {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != dto.UserId)
            {
                return Forbid();
            }
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
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> SubmitTestAttempt([FromBody] SubmitTestAttemptDto dto)
    {
        try
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null || !Guid.TryParse(userClaim.Value, out var userClaimId))
            {
                return Unauthorized("User identifier claim is missing or invalid.");
            }
            var result = await _service.SubmitTestAttemptAsync(dto, userClaimId);
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
    [Authorize(Roles = "STUDENT")]
    public async Task<IActionResult> GetAllByUserId(Guid userId)
    {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != userId)
            {
                return Forbid();
            }
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
    [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
    public async Task<IActionResult> GetAttemptWithScoreSummary(Guid attemptId)
    {
        var (attempt, scoreSummary) = await _service.GetAttemptWithScoreSummaryAsync(attemptId);
        if (attempt == null)
            return NotFound();
        return Ok(new { Attempt = attempt, ScoreSummary = scoreSummary });
    }

    /// <summary>
    /// Get paged test attempts by test id and isPass filter.
    /// </summary>
    [HttpGet("by-test/{testId}/paged")]
    [Authorize(Roles = "INSTRUCTOR,ACADEMIC_MANAGER,STUDENT")]
    public async Task<IActionResult> GetPagedAttemptsByTestIdAndIsPass(
        Guid testId,
        [FromQuery] bool? isPass,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetPagedAttemptsByTestIdAndIsPassAsync(testId, isPass, pageIndex, pageSize);
        return Ok(result);
    }
}}
