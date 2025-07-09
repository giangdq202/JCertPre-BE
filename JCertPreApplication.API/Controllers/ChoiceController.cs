using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Application.Dtos.Choice;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages question choices and answers.
    /// </summary>
    [ApiController]
    [Route("api/choice")]
    [Tags("Choice")]
    [Produces("application/json")]
    public class ChoiceController : ControllerBase
    {
        private readonly IChoiceService _choiceService;

        public ChoiceController(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        /// <summary>
        /// Gets all choices for a question.
        /// </summary>
        /// <param name="questionId">ID of the question.</param>
        /// <returns>List of choices.</returns>
        [HttpGet("question/{questionId}")]
        public async Task<IActionResult> GetByQuestionId(Guid questionId)
        {
            var choices = await _choiceService.GetByQuestionIdAsync(questionId);
            return Ok(choices);
        }

        /// <summary>
        /// Creates a new choice for a question.
        /// </summary>
        /// <param name="questionId">ID of the question.</param>
        /// <param name="dto">Choice creation data.</param>
        /// <returns>The created choice.</returns>
        [HttpPost("question/{questionId}")]
        public async Task<IActionResult> Create(Guid questionId, [FromBody] ChoiceCreateDto dto)
        {
            var created = await _choiceService.CreateAsync(questionId, dto);
            return CreatedAtAction(nameof(GetByQuestionId), new { questionId }, created);
        }

        /// <summary>
        /// Updates multiple choices for a question.
        /// </summary>
        /// <param name="questionId">ID of the question.</param>
        /// <param name="dtos">List of choices to update.</param>
        /// <returns>No content on success.</returns>
        [HttpPut("question/{questionId}")]
        public async Task<IActionResult> UpdateList(Guid questionId, [FromBody] IEnumerable<ChoiceUpdateDto> dtos)
        {
            await _choiceService.UpdateListAsync(questionId, dtos);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific choice.
        /// </summary>
        /// <param name="questionId">ID of the question.</param>
        /// <param name="choiceId">ID of the choice to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("question/{questionId}/choice/{choiceId}")]
        public async Task<IActionResult> Delete(Guid questionId, Guid choiceId)
        {
            await _choiceService.DeleteAsync(questionId, choiceId);
            return NoContent();
        }
    }
}