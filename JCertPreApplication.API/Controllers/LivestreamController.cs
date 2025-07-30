using JCertPreApplication.Application.Dtos.Livestream;
using JCertPreApplication.Application.Features.Livestreams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Temporarily disabled for testing
    public class LivestreamController : ControllerBase
    {
        private readonly ILivestreamService _livestreamService;

        public LivestreamController(ILivestreamService livestreamService)
        {
            _livestreamService = livestreamService;
        }

        /// <summary>
        /// Create a new livestream (Academic Manager only)
        /// </summary>
        [HttpPost]
        // [Authorize(Roles = "Academic Manager")] // Temporarily disabled for testing
        public async Task<IActionResult> CreateLivestream([FromBody] CreateLivestreamDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _livestreamService.CreateLivestreamAsync(createDto);
            return CreatedAtAction(nameof(GetLivestreamById), new { id = result.LivestreamId }, result);
        }

        /// <summary>
        /// Get livestream by ID
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
        /// Update livestream (Academic Manager only)
        /// </summary>
        [HttpPut("{id}")]
        // [Authorize(Roles = "Academic Manager")] // Temporarily disabled for testing
        public async Task<IActionResult> UpdateLivestream(Guid id, [FromBody] UpdateLivestreamDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _livestreamService.UpdateLivestreamAsync(id, updateDto);
            return Ok(result);
        }

        /// <summary>
        /// Delete livestream (Academic Manager only)
        /// </summary>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Academic Manager")] // Temporarily disabled for testing
        public async Task<IActionResult> DeleteLivestream(Guid id)
        {
            await _livestreamService.DeleteLivestreamAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Get livestreams with pagination and filters
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
        /// Get livestreams by course ID
        /// </summary>
        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetLivestreamsByCourse(Guid courseId)
        {
            var result = await _livestreamService.GetLivestreamsByCourseAsync(courseId);
            return Ok(result);
        }

        /// <summary>
        /// Generate join token for livestream
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
        /// Start livestream (Instructor only)
        /// </summary>
        [HttpPost("{id}/start")]
        // [Authorize(Roles = "Instructor")] // Temporarily disabled for testing
        public async Task<IActionResult> StartLivestream(Guid id, [FromQuery] Guid userId)
        {
            if (!await _livestreamService.CanInstructorStartLivestreamAsync(userId, id))
                return Forbid("You don't have permission to start this livestream");

            await _livestreamService.StartLivestreamAsync(id);
            return Ok(new { message = "Livestream started successfully" });
        }

        /// <summary>
        /// Check if user can join livestream
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
