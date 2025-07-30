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
    [Route("api/[controller]")]
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
        /// Gets livestreams with pagination and filters.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLivestreams(
            [FromQuery] Guid? courseId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _livestreamService.GetLivestreamsAsync(courseId, searchTerm, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets livestreams by course ID.
        /// </summary>
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLivestreamsByCourse(Guid courseId)
        {
            var result = await _livestreamService.GetLivestreamsByCourseAsync(courseId);
            return Ok(result);
        }

        /// <summary>
        /// Generates join token for livestream.
        /// </summary>
        [HttpGet("{id}/join-token")]
        public async Task<IActionResult> GetJoinToken(Guid id, [FromQuery] Guid userId)
        {
            if (!await _livestreamService.CanUserJoinLivestreamAsync(userId, id))
                return Forbid("You don't have permission to join this livestream");

            var result = await _livestreamService.GenerateJoinTokenAsync(userId, id);
            return Ok(result);
        }

        /// <summary>
        /// Starts a livestream.
        /// </summary>
        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartLivestream(Guid id, [FromQuery] Guid userId)
        {
            if (!await _livestreamService.CanInstructorStartLivestreamAsync(userId, id))
                return Forbid("You don't have permission to start this livestream");

            await _livestreamService.StartLivestreamAsync(id);
            return Ok(new { message = "Livestream started successfully" });
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

        /// <summary>
        /// Gets livestreams by user ID.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetLivestreamsByUser(Guid userId)
        {
            var livestreams = await _livestreamService.GetLivestreamsByUserAsync(userId);
            return Ok(livestreams);
        }

        /// <summary>
        /// Gets livestream timetable by user ID.
        /// </summary>
        [HttpGet("user/{userId}/timetable")]
        public async Task<IActionResult> GetLivestreamTimetableByUser(Guid userId)
        {
            var timetable = await _livestreamService.GetLivestreamTimetableByUserAsync(userId);
            return Ok(timetable);
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
