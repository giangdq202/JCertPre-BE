using JCertPreApplication.Application.Dtos.AttemptAnswer;
using JCertPreApplication.Application.Features.AttemptAnswers;
using Microsoft.AspNetCore.Mvc;

[Route("api/attempt-answers")]
[ApiController]
public class AttemptAnswerController : ControllerBase
{
    private readonly IAttemptAnswerService _service;

    public AttemptAnswerController(IAttemptAnswerService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all attempt answers by attemptId.
    /// </summary>
    [HttpGet("by-attempt/{attemptId}")]
    public async Task<IActionResult> GetAllByAttemptId(Guid attemptId)
    {
        var result = await _service.GetAllByAttemptIdAsync(attemptId);
        return Ok(result);
    }

    /// <summary>
    /// Add or update one or multiple attempt answers.
    /// </summary>
    [HttpPost("add-or-update")]
    public async Task<IActionResult> AddOrUpdateAnswers([FromBody] List<CreateAttemptAnswerDto> dtos)
    {
        var result = await _service.AddOrUpdateAnswersAsync(dtos);
        return Ok(result);
    }
}