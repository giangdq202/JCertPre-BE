using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Application.Features.StudentProfile;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/student-profile")]
    [ApiController]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileService _studentProfileService;

        public StudentProfileController(IStudentProfileService studentProfileService)
        {
            _studentProfileService = studentProfileService ?? throw new ArgumentNullException(nameof(studentProfileService));
        }

        [HttpPost("create")]
        public async Task<ActionResult<Domain.Entities.StudentProfile>> CreateStudentProfile([FromQuery] Guid userId, [FromQuery] string currentLevel, [FromQuery] string learningGoals)
        {
            var profile = await _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Domain.Entities.StudentProfile>> GetStudentProfile(Guid userId)
        {
            var profile = await _studentProfileService.GetStudentProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut("update/{userId}")]
        public async Task<ActionResult<Domain.Entities.StudentProfile>> UpdateStudentProfile(Guid userId, [FromQuery] string currentLevel, [FromQuery] string learningGoals)
        {
            var profile = await _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteStudentProfile(Guid userId)
        {
            var result = await _studentProfileService.DeleteStudentProfileAsync(userId);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
