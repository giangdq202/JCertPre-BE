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
    /// Update choiceId for a specific answer.
    /// </summary>
    [HttpPut("update-choice")]
    public async Task<IActionResult> UpdateChoice([FromBody] UpdateAttemptAnswerDto dto)
    {
        var result = await _service.UpdateChoiceAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Add a new attempt answer.
    /// </summary>
    [HttpPost("add")]
    public async Task<IActionResult> AddAnswer([FromBody] CreateAttemptAnswerDto dto)
    {
        var result = await _service.AddAnswerAsync(dto);
        return Ok(result);
    }
}