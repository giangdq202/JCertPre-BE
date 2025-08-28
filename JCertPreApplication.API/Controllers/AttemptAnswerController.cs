using JCertPreApplication.Application.Dtos.AttemptAnswer;
using JCertPreApplication.Application.Features.AttemptAnswers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages attempt answer operations.
    /// </summary>
    [Route("api/attempt-answers")]
    [ApiController]
    [Tags("AttemptAnswers")]
    [Produces("application/json")]
    [Authorize]
    public class AttemptAnswerController : ControllerBase
{
    private readonly IAttemptAnswerService _service;

    public AttemptAnswerController(IAttemptAnswerService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Get all attempt answers by attemptId.
    /// </summary>
    [HttpGet("by-attempt/{attemptId}")]
    [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetAllByAttemptId(Guid attemptId)
    {
        var result = await _service.GetAllByAttemptIdAsync(attemptId);
        return Ok(result);
    }

    /// <summary>
    /// Add or update one or multiple attempt answers.
    /// </summary>
    [HttpPost("add-or-update")]
    [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> AddOrUpdateAnswers([FromBody] List<CreateAttemptAnswerDto> dtos)
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null || !Guid.TryParse(userClaim.Value, out var userClaimId))
            {
                return Unauthorized("User identifier claim is missing or invalid.");
            }
            var result = await _service.AddOrUpdateAnswersAsync(dtos, userClaimId);
            return Ok(result);
        }
}}
