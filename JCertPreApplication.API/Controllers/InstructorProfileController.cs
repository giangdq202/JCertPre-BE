using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Features.InstructorProfile;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages instructor profile information.
    /// </summary>
    [Route("api/instructor-profile")]
    [ApiController]
    [Tags("InstructorProfile")]
    [Produces("application/json")]
    public class InstructorProfileController : ControllerBase
    {
        private readonly IInstructorProfileService _instructorProfileService;

        public InstructorProfileController(IInstructorProfileService instructorProfileService)
        {
            _instructorProfileService = instructorProfileService ?? throw new ArgumentNullException(nameof(instructorProfileService));
        }

        /// <summary>
        /// Creates a new instructor profile.
        /// </summary>
        /// <param name="userId">Instructor user ID.</param>
        /// <param name="introduction">Brief introduction.</param>
        /// <param name="experience">Teaching experience.</param>
        /// <param name="teachingStyle">Teaching methodology.</param>
        /// <returns>Created instructor profile.</returns>
        [HttpPost("create")]
        public async Task<ActionResult> CreateInstructorProfile([FromQuery] Guid userId, [FromQuery] string introduction, [FromQuery] string? experience, [FromQuery] string? teachingStyle)
        {
            var profile = await _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Gets an instructor's profile.
        /// </summary>
        /// <param name="userId">Instructor user ID.</param>
        /// <returns>Instructor profile details.</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<InstructorProfileDto>> GetInstructorProfile(Guid userId)
        {
            var profile = await _instructorProfileService.GetInstructorProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Updates an instructor's profile.
        /// </summary>
        /// <param name="userId">Instructor user ID.</param>
        /// <param name="introduction">Updated introduction.</param>
        /// <param name="experience">Updated experience.</param>
        /// <param name="teachingStyle">Updated teaching style.</param>
        /// <returns>Updated instructor profile.</returns>
        [HttpPut("update/{userId}")]
        public async Task<ActionResult<InstructorProfileDto>> UpdateInstructorProfile(Guid userId, [FromQuery] string introduction, [FromQuery] string? experience, [FromQuery] string? teachingStyle)
        {
            var profile = await _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Deletes an instructor's profile.
        /// </summary>
        /// <param name="userId">Instructor user ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteInstructorProfile(Guid userId)
        {
            var result = await _instructorProfileService.DeleteInstructorProfileAsync(userId);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
