using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Application.Features.Questions.Dtos;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
            var result = questions.Select(q => new QuestionReadDto
            {
                QuestionId = q.questionId,
                QuestionText = q.questionText,
                QuestionType = q.questionType,
                Explanation = q.explanation,
                TagId = q.tagId
            });
            return Ok(result);
        }

        /// <summary>
        /// Get a question by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var q = await _questionService.GetByIdAsync(id);
            if (q == null)
                return NotFound();
            var dto = new QuestionReadDto
            {
                QuestionId = q.questionId,
                QuestionText = q.questionText,
                QuestionType = q.questionType,
                Explanation = q.explanation,
                TagId = q.tagId
            };
            return Ok(dto);
        }

        /// <summary>
        /// Create a new question.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] QuestionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = new Question
            {
                questionId = Guid.NewGuid(),
                questionText = dto.QuestionText,
                questionType = dto.QuestionType,
                explanation = dto.Explanation,
                tagId = dto.TagId
            };
            var created = await _questionService.CreateAsync(question);
            var result = new QuestionReadDto
            {
                QuestionId = created.questionId,
                QuestionText = created.questionText,
                QuestionType = created.questionType,
                Explanation = created.explanation,
                TagId = created.tagId
            };
            return CreatedAtAction(nameof(GetById), new { id = result.QuestionId }, result);
        }

        /// <summary>
        /// Update an existing question.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] QuestionUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = new Question
            {
                questionId = dto.QuestionId,
                questionText = dto.QuestionText,
                questionType = dto.QuestionType,
                explanation = dto.Explanation,
                tagId = dto.TagId
            };
            await _questionService.UpdateAsync(question);
            return NoContent();
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
        /// Get all questions with their related details (choices, attachments, etc.).
        /// </summary>
        [HttpGet("details")]
        public async Task<IActionResult> GetQuestionsWithDetails()
        {
            var questions = await _questionService.GetQuestionsWithDetailsAsync();
            var result = questions.Select(q => new QuestionReadDto
            {
                QuestionId = q.questionId,
                QuestionText = q.questionText,
                QuestionType = q.questionType,
                Explanation = q.explanation,
                TagId = q.tagId
            });
            return Ok(result);
        }

        /// <summary>
        /// Get paginated questions with optional search.
        /// </summary>
        [HttpGet("paging")]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            Expression<Func<Question, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                predicate = q => q.questionText.Contains(search);
            }

            var result = await _questionService.GetPagingAsync(predicate, "Choices,QuestionAttachments,Tag", pageIndex, pageSize);

            // Map to DTOs
            var dtoResult = new Pagination<QuestionReadDto>
            {
                PageIndex = result.PageIndex,
                PageSize = result.PageSize,
                TotalItemsCount = result.TotalItemsCount,
                Items = result.Items.Select(q => new QuestionReadDto
                {
                    QuestionId = q.questionId,
                    QuestionText = q.questionText,
                    QuestionType = q.questionType,
                    Explanation = q.explanation,
                    TagId = q.tagId
                }).ToList()
            };

            return Ok(dtoResult);
        }
    }
}