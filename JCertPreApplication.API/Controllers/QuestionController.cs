using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Features.Questions;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages test questions and their details.
    /// </summary>
    [ApiController]
    [Route("api/question")]
    [Tags("Questions")]
    [Produces("application/json")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        /// <summary>
        /// Gets all questions.
        /// </summary>
        /// <returns>List of all questions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _questionService.GetAllAsync();
            return Ok(questions);
        }

        /// <summary>
        /// Gets a question by ID.
        /// </summary>
        /// <param name="id">Question ID.</param>
        /// <returns>Question details.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null)
                return NotFound();
            return Ok(question);
        }

        /// <summary>
        /// Creates a new question.
        /// </summary>
        /// <param name="dto">Question creation data.</param>
        /// <returns>Created question details.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _questionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing question.
        /// </summary>
        /// <param name="id">Question ID.</param>
        /// <param name="dto">Question update data.</param>
        /// <returns>Updated question details.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _questionService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a question.
        /// </summary>
        /// <param name="id">Question ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _questionService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets paginated questions with their choices and attachments.
        /// </summary>
        /// <param name="pageIndex">Page number (starts from 1).</param>
        /// <param name="pageSize">Items per page.</param>
        /// <param name="search">Optional search term.</param>
        /// <returns>Paginated list of questions with details.</returns>
        [HttpGet("paging-details")]
        public async Task<IActionResult> GetPagingWithDetails(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _questionService.GetPaginatedWithDetailsAsync(search, pageIndex, pageSize);
            return Ok(result);
        }
    }
}