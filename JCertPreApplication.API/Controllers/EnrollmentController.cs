using JCertPreApplication.Application.Dtos.Enrollment;
using JCertPreApplication.Application.Features.Enrollment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles course enrollment operations, including user enrollment, enrollment status checks, and enrollment management.
    /// </summary>
    [Route("api/enrollments")]
    [ApiController]
    [Tags("Enrollment")]
    [Produces("application/json")]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService ?? throw new ArgumentNullException(nameof(enrollmentService));
        }

        /// <summary>
        /// Enrolls a user in a course using provided user ID and course ID.
        /// </summary>
        /// <param name="request">Enrollment request containing user ID and course ID.</param>
        /// <returns>Enrollment details including transaction information.</returns>
        [HttpPost("enroll")]
        [Authorize(Roles = "STUDENT,ACADEMIC_MANAGER")]
        public async Task<IActionResult> EnrollInCourse([FromBody] EnrollmentRequestDto request)
        {
            var result = await _enrollmentService.EnrollUserAsync(request.UserId, request.CourseId);
            return Ok(result);
        }

        /// <summary>
        /// Enrolls the current authenticated user in a course.
        /// </summary>
        /// <param name="request">Self enrollment request containing course ID.</param>
        /// <returns>Enrollment details including transaction information.</returns>
        [HttpPost("enroll-self")]
        [Authorize(Roles = "STUDENT,ACADEMIC_MANAGER")]
        public async Task<IActionResult> EnrollSelfInCourse([FromBody] SelfEnrollmentRequestDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _enrollmentService.EnrollUserAsync(userId, request.CourseId);
            return Ok(result);
        }

        /// <summary>
        /// Checks if the current user is enrolled in a specific course.
        /// </summary>
        /// <param name="courseId">The course ID to check enrollment status for.</param>
        /// <returns>The enrollment status result.</returns>
        [HttpGet("check/{courseId}")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> CheckEnrollmentStatus(Guid courseId)
        {
            var userId = GetCurrentUserId();
            var isEnrolled = await _enrollmentService.IsUserEnrolledAsync(userId, courseId);
            return Ok(new { 
                isEnrolled = isEnrolled,
                message = isEnrolled ? "User is enrolled in this course" : "User is not enrolled in this course"
            });
        }

        /// <summary>
        /// Retrieves all enrollments for the current authenticated user.
        /// </summary>
        /// <returns>List of user enrollments with course details.</returns>
        [HttpGet("my-enrollments")]
        [Authorize(Roles = "STUDENT,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = GetCurrentUserId();
            var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
            return Ok(enrollments);
        }

        /// <summary>
        /// Unenrolls the current user from a course.
        /// </summary>
        /// <param name="courseId">The course ID to unenroll from.</param>
        /// <returns>The unenrollment result.</returns>
        [HttpDelete("unenroll/{courseId}")]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> UnenrollFromCourse(Guid courseId)
        {
            var userId = GetCurrentUserId();
            var result = await _enrollmentService.UnenrollUserAsync(userId, courseId);
            
            if (result)
            {
                return Ok(new { success = true, message = "Successfully unenrolled from course" });
            }
            else
            {
                return NotFound(new { success = false, message = "Enrollment not found" });
            }
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