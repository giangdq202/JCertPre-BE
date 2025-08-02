using JCertPreApplication.Application.Dtos.Livestream;
using JCertPreApplication.Application.Features.Livestreams;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages livestream operations.
    /// </summary>
    [ApiController]
    [Route("api/livestreams")]
    [Tags("Livestreams")]
    [Produces("application/json")]
    public class LivestreamController : ControllerBase
    {
        private readonly ILivestreamService _livestreamService;

        public LivestreamController(ILivestreamService livestreamService)
        {
            _livestreamService = livestreamService;
        }

        /// <summary>
        /// Creates a new livestream.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLivestream([FromBody] CreateLivestreamDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _livestreamService.CreateLivestreamAsync(createDto);
            return CreatedAtAction(nameof(GetLivestreamById), new { id = result.LivestreamId }, result);
        }

        /// <summary>
        /// Gets livestream by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLivestreamById(Guid id)
        {
            var result = await _livestreamService.GetLivestreamByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Updates a livestream.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLivestream(Guid id, [FromBody] UpdateLivestreamDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _livestreamService.UpdateLivestreamAsync(id, updateDto);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a livestream.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivestream(Guid id)
        {
            await _livestreamService.DeleteLivestreamAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets livestreams with comprehensive filtering and pagination.
        /// Supports filtering by course, user, date range, and status.
        /// Can return regular list or timetable format.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLivestreams(
            [FromQuery] Guid? courseId = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool? timetableFormat = false,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            // If userId is provided and timetableFormat is requested, return timetable
            if (userId.HasValue && timetableFormat == true)
            {
                var timetable = await _livestreamService.GetLivestreamTimetableByUserAsync(userId.Value);
                return Ok(timetable);
            }

            // If userId is provided but not timetable format, get user's livestreams
            if (userId.HasValue)
            {
                var userLivestreams = await _livestreamService.GetLivestreamsByUserAsync(userId.Value);
                return Ok(userLivestreams);
            }

            // If courseId is provided, get course livestreams (no pagination for simplicity)
            if (courseId.HasValue && pageIndex == 1 && pageSize == 10 && !startDate.HasValue && !endDate.HasValue)
            {
                var courseLivestreams = await _livestreamService.GetLivestreamsByCourseAsync(courseId.Value);
                return Ok(courseLivestreams);
            }

            // Default: get all livestreams with pagination and filters
            var result = await _livestreamService.GetLivestreamsAsync(courseId, startDate, endDate, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Generates join token for livestream.
        /// </summary>
        [HttpGet("{id}/join-token")]
        public async Task<IActionResult> GetJoinToken(Guid id, [FromQuery] Guid userId)
        {
            if (!await _livestreamService.CanUserJoinLivestreamAsync(userId, id))
                return Forbid("You don't have permission to join this livestream or the livestream is not currently live");

            var result = await _livestreamService.GenerateJoinTokenAsync(userId, id);
            return Ok(result);
        }

        /// <summary>
        /// Checks if user can join livestream.
        /// </summary>
        [HttpGet("{id}/can-join")]
        public async Task<IActionResult> CanJoinLivestream(Guid id, [FromQuery] Guid userId)
        {
            var canJoin = await _livestreamService.CanUserJoinLivestreamAsync(userId, id);
            return Ok(new { canJoin });
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }
}
