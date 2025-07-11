using JCertPreApplication.Application.Dtos.Enrollment;
using JCertPreApplication.Application.Features.Enrollment;
using JCertPreApplication.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(IEnrollmentService enrollmentService, ILogger<EnrollmentController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        /// <summary>
        /// Enroll a user in a course (Admin function)
        /// </summary>
        /// <param name="request">Enrollment request containing user ID and course ID</param>
        /// <returns>Enrollment details</returns>
        [HttpPost("enroll")]
        public async Task<ActionResult<EnrollmentResponseDto>> EnrollInCourse([FromBody] EnrollmentRequestDto request)
        {
            try
            {
                var result = await _enrollmentService.EnrollUserAsync(request.UserId, request.CourseId);
                
                _logger.LogInformation("User {UserId} successfully enrolled in course {CourseId}", request.UserId, request.CourseId);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling user {UserId} in course {CourseId}", request.UserId, request.CourseId);
                throw;
            }
        }

        /// <summary>
        /// Enroll current user in a course (Self enrollment)
        /// </summary>
        /// <param name="courseId">Course ID to enroll in</param>
        /// <returns>Enrollment details</returns>
        [HttpPost("enroll-self/{courseId}")]
        public async Task<ActionResult<EnrollmentResponseDto>> EnrollSelfInCourse(Guid courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _enrollmentService.EnrollUserAsync(userId, courseId);
                
                _logger.LogInformation("User {UserId} successfully enrolled in course {CourseId}", userId, courseId);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling user {UserId} in course {CourseId}", GetCurrentUserId(), courseId);
                throw;
            }
        }

        /// <summary>
        /// Check if current user is enrolled in a specific course
        /// </summary>
        /// <param name="courseId">Course ID to check</param>
        /// <returns>True if enrolled, false otherwise</returns>
        [HttpGet("check/{courseId}")]
        public async Task<ActionResult<bool>> CheckEnrollmentStatus(Guid courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isEnrolled = await _enrollmentService.IsUserEnrolledAsync(userId, courseId);
                
                return Ok(isEnrolled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enrollment status for course {CourseId}", courseId);
                throw;
            }
        }

        /// <summary>
        /// Get all enrollments for current user
        /// </summary>
        /// <returns>List of user enrollments</returns>
        [HttpGet("my-enrollments")]
        public async Task<ActionResult<IEnumerable<EnrollmentResponseDto>>> GetMyEnrollments()
        {
            try
            {
                var userId = GetCurrentUserId();
                var enrollments = await _enrollmentService.GetUserEnrollmentsAsync(userId);
                
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enrollments for user");
                throw;
            }
        }

        /// <summary>
        /// Unenroll current user from a course
        /// </summary>
        /// <param name="courseId">Course ID to unenroll from</param>
        /// <returns>Success status</returns>
        [HttpDelete("unenroll/{courseId}")]
        public async Task<ActionResult<bool>> UnenrollFromCourse(Guid courseId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _enrollmentService.UnenrollUserAsync(userId, courseId);
                
                if (result)
                {
                    _logger.LogInformation("User {UserId} successfully unenrolled from course {CourseId}", userId, courseId);
                    return Ok(new { success = true, message = "Successfully unenrolled from course" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Enrollment not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unenrolling user from course {CourseId}", courseId);
                throw;
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