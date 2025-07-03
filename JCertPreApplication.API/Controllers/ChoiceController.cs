using JCertPreApplication.Application.Features.Choices;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChoiceController : ControllerBase
    {
        private readonly IChoiceService _choiceService;

        public ChoiceController(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetByQuestionId(Guid questionId)
        {
            var choices = await _choiceService.GetByQuestionIdAsync(questionId);
            return Ok(choices);
        }

        [HttpPost("question/{questionId}")]
        public async Task<IActionResult> Create(Guid questionId, [FromBody] ChoiceCreateDto dto)
        {
            var created = await _choiceService.CreateAsync(questionId, dto);
            return CreatedAtAction(nameof(GetByQuestionId), new { questionId }, created);
        }

        [HttpPut("question/{questionId}")]
        public async Task<IActionResult> UpdateList(Guid questionId, [FromBody] IEnumerable<ChoiceUpdateDto> dtos)
        {
            await _choiceService.UpdateListAsync(questionId, dtos);
            return NoContent();
        }

        [HttpDelete("question/{questionId}/choice/{choiceId}")]
        public async Task<IActionResult> Delete(Guid questionId, Guid choiceId)
        {
            await _choiceService.DeleteAsync(questionId, choiceId);
            return NoContent();
        }
    }
}