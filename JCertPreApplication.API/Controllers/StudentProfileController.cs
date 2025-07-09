using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Features.StudentProfile;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages student profile information.
    /// </summary>
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

        /// <summary>
        /// Creates a new student profile.
        /// </summary>
        /// <param name="userId">Student user ID.</param>
        /// <param name="currentLevel">Current Japanese level.</param>
        /// <param name="learningGoals">Study objectives.</param>
        /// <returns>Created student profile.</returns>
        [HttpPost("create")]
        public async Task<ActionResult> CreateStudentProfile([FromQuery] Guid userId, [FromQuery] string currentLevel, [FromQuery] string learningGoals)
        {
            var profile = await _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Gets a student's profile.
        /// </summary>
        /// <param name="userId">Student user ID.</param>
        /// <returns>Student profile details.</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<StudentProfileDto>> GetStudentProfile(Guid userId)
        {
            var profile = await _studentProfileService.GetStudentProfileAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Updates a student's profile.
        /// </summary>
        /// <param name="userId">Student user ID.</param>
        /// <param name="currentLevel">Updated Japanese level.</param>
        /// <param name="learningGoals">Updated study objectives.</param>
        /// <returns>Updated student profile.</returns>
        [HttpPut("update/{userId}")]
        public async Task<ActionResult<StudentProfileDto>> UpdateStudentProfile(Guid userId, [FromQuery] string currentLevel, [FromQuery] string learningGoals)
        {
            var profile = await _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        /// <summary>
        /// Deletes a student's profile.
        /// </summary>
        /// <param name="userId">Student user ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteStudentProfile(Guid userId)
        {
            var result = await _studentProfileService.DeleteStudentProfileAsync(userId);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
