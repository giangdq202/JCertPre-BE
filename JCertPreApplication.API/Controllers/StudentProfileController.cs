using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Features.StudentProfile;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/student-profile")]
    [ApiController]
    [Tags("StudentProfile")]
    [Produces("application/json")]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileService _studentProfileService;

        public StudentProfileController(IStudentProfileService studentProfileService)
        {
            _studentProfileService = studentProfileService ?? throw new ArgumentNullException(nameof(studentProfileService));
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateStudentProfile([FromQuery] Guid userId, [FromQuery] string currentLevel, [FromQuery] string learningGoals)
        {
            var profile = await _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentProfileDto>> GetStudentProfile(Guid userId)
        {
            var profile = await _studentProfileService.GetStudentProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut("update/{userId}")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentProfileDto>> UpdateStudentProfile(Guid userId, [FromQuery] string currentLevel, [FromQuery] string learningGoals)
        {
            var profile = await _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpDelete("delete/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudentProfile(Guid userId)
        {
            var result = await _studentProfileService.DeleteStudentProfileAsync(userId);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
