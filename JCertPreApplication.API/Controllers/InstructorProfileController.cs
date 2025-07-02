using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Conversation;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace JCertPreApplication.API.Controllers
{
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

        [HttpPost("create")]
        [ProducesResponseType(typeof(InstructorProfileDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateInstructorProfile([FromQuery] Guid userId, [FromQuery] string introduction, [FromQuery] string? experience, [FromQuery] string? teachingStyle)
        {
            var profile = await _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(InstructorProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InstructorProfileDto>> GetInstructorProfile(Guid userId)
        {
            var profile = await _instructorProfileService.GetInstructorProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut("update/{userId}")]
        [ProducesResponseType(typeof(InstructorProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InstructorProfileDto>> UpdateInstructorProfile(Guid userId, [FromQuery] string introduction, [FromQuery] string? experience, [FromQuery] string? teachingStyle)
        {
            var profile = await _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpDelete("delete/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInstructorProfile(Guid userId)
        {
            var result = await _instructorProfileService.DeleteInstructorProfileAsync(userId);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
