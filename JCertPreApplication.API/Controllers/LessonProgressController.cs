using JCertPreApplication.Application.Dtos.LessonProgress;
using JCertPreApplication.Application.Features.LessonProgresses;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Controller for managing LessonProgress CRUD operations.
    /// </summary>
    [ApiController]
    [Route("api/lesson-progress")]
    [Tags("LessonProgress")]
    [Produces("application/json")]
    public class LessonProgressController : ControllerBase
    {
        private readonly ILessonProgressService _service;

        public LessonProgressController(ILessonProgressService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all lesson progress records for a user in a course.
        /// </summary>
        [HttpGet("by-user-course")]
        public async Task<IActionResult> GetByUserAndCourse([FromQuery] Guid userId, [FromQuery] Guid courseId)
        {
            var result = await _service.GetByUserAndCourseAsync(userId, courseId);
            return Ok(result);
        }

        /// <summary>
        /// Get a lesson progress record by user and lesson.
        /// </summary>
        [HttpGet("by-user-lesson")]
        public async Task<IActionResult> GetByUserAndLesson([FromQuery] Guid userId, [FromQuery] Guid lessonId)
        {
            var result = await _service.GetByUserAndLessonAsync(userId, lessonId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Create a new lesson progress record.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLessonProgressDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUserAndLesson), new { userId = result.UserId, lessonId = result.LessonId }, result);
        }

        /// <summary>
        /// Update a lesson progress record.
        /// </summary>
        [HttpPut("{progressId:guid}")]
        public async Task<IActionResult> Update(Guid progressId, [FromBody] UpdateLessonProgressDto dto)
        {
            var result = await _service.UpdateAsync(progressId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Delete a lesson progress record.
        /// </summary>
        [HttpDelete("{progressId:guid}")]
        public async Task<IActionResult> Delete(Guid progressId)
        {
            await _service.DeleteAsync(progressId);
            return NoContent();
        }

        /// <summary>
        /// Get the current user's overall completion rate for a course.
        /// </summary>
        [HttpGet("completion-rate")]
        public async Task<IActionResult> GetUserCourseCompletionRate([FromQuery] Guid userId, [FromQuery] Guid courseId)
        {
            var rate = await _service.GetUserCourseCompletionRateAsync(userId, courseId);
            return Ok(rate);
        }
    }
}