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
        /// Updates a specific choice.
        /// </summary>
        /// <param name="choiceId">ID of the choice to update.</param>
        /// <param name="dto">Choice update data.</param>
        /// <returns>No content on success.</returns>
        [HttpPut("choice/{choiceId}")]
        public async Task<IActionResult> Update(Guid choiceId, [FromBody] ChoiceUpdateDto dto)
        {
            await _choiceService.UpdateAsync(choiceId, dto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific choice.
        /// </summary>
        /// <param name="choiceId">ID of the choice to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("choice/{choiceId}")]
        public async Task<IActionResult> Delete(Guid choiceId)
        {
            await _choiceService.DeleteAsync(choiceId);
            return NoContent();
        }
    }
}