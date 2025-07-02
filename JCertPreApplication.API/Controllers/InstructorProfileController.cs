using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/instructor-profile")]
    [ApiController]
    public class InstructorProfileController : ControllerBase
    {
        private readonly IInstructorProfileService _instructorProfileService;

        public InstructorProfileController(IInstructorProfileService instructorProfileService)
        {
            _instructorProfileService = instructorProfileService ?? throw new ArgumentNullException(nameof(instructorProfileService));
        }

        [HttpPost("create")]
        public async Task<ActionResult<InstructorProfile>> CreateInstructorProfile([FromQuery] Guid userId, [FromQuery] string introduction, [FromQuery] string? experience, [FromQuery] string? teachingStyle)
        {
            var profile = await _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult<InstructorProfile>> GetInstructorProfile(Guid userId)
        {
            var profile = await _instructorProfileService.GetInstructorProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut("update/{userId}")]
        public async Task<ActionResult<InstructorProfile>> UpdateInstructorProfile(Guid userId, [FromQuery] string introduction, [FromQuery] string? experience, [FromQuery] string? teachingStyle)
        {
            var profile = await _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteInstructorProfile(Guid userId)
        {
            var result = await _instructorProfileService.DeleteInstructorProfileAsync(userId);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
