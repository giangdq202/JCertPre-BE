using JCertPreApplication.Application.Dtos.Feedback;
using JCertPreApplication.Application.Features.Feedbacks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages feedback operations.
    /// </summary>
    [Route("api/feedbacks")]
    [ApiController]
    [Tags("Feedbacks")]
    [Produces("application/json")]
    [Authorize]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }

        /// <summary>
        /// Get paginated feedbacks for a course, ordered by createdAt descending.
        /// </summary>
        [HttpGet("course/{courseId:guid}")]
        public async Task<IActionResult> GetPagingByCourseId(Guid courseId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _feedbackService.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Create a feedback for a user and course.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _feedbackService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPagingByCourseId), new { courseId = created.CourseId }, created);
        }

        /// <summary>
        /// Update a feedback by user and course.
        /// </summary>
        [HttpPut("{userId:guid}/{courseId:guid}")]
        public async Task<IActionResult> Update(Guid userId, Guid courseId, [FromBody] UpdateFeedbackDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _feedbackService.UpdateAsync(userId, courseId, dto);
            return Ok(updated);
        }

        /// <summary>
        /// Delete a feedback by user and course.
        /// </summary>
        [HttpDelete("{userId:guid}/{courseId:guid}")]
        public async Task<IActionResult> Delete(Guid userId, Guid courseId)
        {
            await _feedbackService.DeleteAsync(userId, courseId);
            return NoContent();
        }

        /// <summary>
        /// Get average rating for a course.
        /// </summary>
        [HttpGet("course/{courseId:guid}/average-rating")]
        public async Task<IActionResult> GetCourseAverageRating(Guid courseId)
        {
            var avg = await _feedbackService.GetCourseAverageRatingAsync(courseId);
            return Ok(avg);
        }
    }
}