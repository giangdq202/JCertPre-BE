using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages question operations including creation, updates, and retrieval.
    /// </summary>
    [Route("api/questions")]
    [ApiController]
    [Tags("Questions")]
    [Produces("application/json")]
    [Authorize]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
        }

        /// <summary>
        /// Get a question by its ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var dto = await _questionService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Create a new question.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Create([FromForm] CreateQuestionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _questionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Generate a new question using AI based on JLPT criteria. Returns formatted content only, does not save to database.
        /// </summary>
        [HttpPost("generate-ai")]
        [Consumes("application/json")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> GenerateWithAI([FromBody] GenerateQuestionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var generatedQuestion = await _questionService.GenerateQuestionWithAIAsync(dto);
            return Ok(generatedQuestion);
        }

        /// <summary>
        /// Generate an explanation for a question using AI. Takes question text and choices, returns explanation in Vietnamese.
        /// </summary>
        [HttpPost("generate-explanation")]
        [Consumes("application/json")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> GenerateExplanation([FromBody] ExplanationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var explanation = await _questionService.GenerateExplanationAsync(dto);
            return Ok(explanation);
        }

        /// <summary>
        /// Update an existing question.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateQuestionDto dto)
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
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _questionService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Get paginated questions with details (choices, attachments), filterable by subcontent fields.
        /// </summary>
        [HttpGet("paging-details")]
        [Authorize(Roles = "INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetPagingWithDetails(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] ContentName? contentName = null,
            [FromQuery] CourseLevel? level = null,
            [FromQuery] SubContentName? subContentName = null)
        {
            var dto = await _questionService.GetPaginatedWithDetailsAsync(search, pageIndex, pageSize, contentName, level, subContentName);
            return Ok(dto);
        }

        /// <summary>
        /// Get a question by its ID for test by the ID.
        /// </summary>
        [HttpGet("test/{id:guid}")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> GetByIdForTest(Guid id)
        {
            var dto = await _questionService.GetByIdForTestAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Get paginated active questions with details (choices, attachments), filterable by subcontent fields.
        /// </summary>
        [HttpGet("paging-details/active")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetPagingActiveWithDetails(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] ContentName? contentName = null,
            [FromQuery] CourseLevel? level = null,
            [FromQuery] SubContentName? subContentName = null,
            [FromQuery] QuestionDifficulty? difficulty = null)
        {
            var dto = await _questionService.GetPaginatedActiveWithDetailsAsync(search, pageIndex, pageSize, contentName, level, subContentName, difficulty);
            return Ok(dto);
        }

        /// <summary>
        /// Import questions from a file.
        /// </summary>
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> ImportQuestions([FromForm] ImportQuestionsRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _questionService.ImportQuestionsAsync(dto);

            if (result.FailedCount > 0 && !string.IsNullOrEmpty(result.FailedFileUrl))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(Path.GetTempPath(), result.FailedFileUrl));
                System.IO.File.Delete(Path.Combine(Path.GetTempPath(), result.FailedFileUrl));

                // Add summary info to response headers
                Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Append("X-Success-Count", result.SuccessCount.ToString());
                Response.Headers.Append("X-Failed-Count", result.FailedCount.ToString());

                return File(fileBytes, "application/json", "import_failed.json");
            }

            // No failed questions, return summary as JSON
            return Ok(new
            {
                totalCount = result.TotalCount,
                successCount = result.SuccessCount,
                failedCount = result.FailedCount
            });
        }

        /// <summary>
        /// Get a random list of questions with choices and explanation by subcontent.
        /// </summary>
        [HttpPost("random")]
        [Authorize (Roles = "STUDENT")]
        public async Task<IActionResult> GetRandomQuestionsWithChoices([FromBody] GetRandomQuestionsRequestDto requestDto)
        {
            // Validate input model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call service and return result
            var questions = await _questionService.GetRandomQuestionsWithChoicesAsync(requestDto);
            return Ok(questions);
        }
    }
}
