using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Features.Questions;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// API endpoints for managing Question entities.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        /// <summary>
        /// Get all questions.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _questionService.GetAllAsync();
            return Ok(questions);
        }

        /// <summary>
        /// Get a question by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null)
                return NotFound();
            return Ok(question);
        }

        /// <summary>
        /// Create a new question.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _questionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update an existing question.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _questionService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a question by its ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _questionService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Get paginated questions with details (choices, attachments), not including tag.
        /// </summary>
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