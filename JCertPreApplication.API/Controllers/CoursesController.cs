using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages course operations and instructor assignments.
    /// Supports CourseType enum with values: Personal (0), Public (1).
    /// Uses file upload for thumbnails, ThumbnailUrl is deprecated in create operations.
    /// </summary>
    [Route("api/course")]
    [ApiController]
    [Tags("Courses")]
    [Produces("application/json")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        }

        /// <summary>
        /// Gets courses with filtering and pagination.
        /// </summary>
        /// <param name="queryParameters">Query parameters for filtering courses by various criteria including CourseType (Personal=0, Public=1)</param>
        /// <returns>Paginated list of courses</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourses([FromQuery] CourseQueryParameters queryParameters)
        {
            var result = await _courseService.GetCoursesWithPaginationAsync(queryParameters);
            return Ok(result);
        }

        /// <summary>
        /// Gets course by ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(course);
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="createCourseDto">Course creation data including CourseType (Personal=0, Public=1). Use ThumbnailFile for image upload, ThumbnailUrl is no longer supported.</param>
        /// <returns>Created course details</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseDto createCourseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        /// <param name="id">Course ID to update</param>
        /// <param name="updateCourseDto">Course update data. CourseType can be changed to Personal=0 or Public=1. Use ThumbnailFile for new image upload.</param>
        /// <returns>Updated course details</returns>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromForm] UpdateCourseDto updateCourseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.UpdateCourseAsync(id, updateCourseDto);
            return Ok(course);
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates course status.
        /// </summary>
        /// <param name="id">Course ID to update</param>
        /// <param name="status">New course status (Draft, Published, Archived)</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] CourseStatus status)
        {
            await _courseService.UpdateCourseStatusAsync(id, status);
            return NoContent();
        }

        /// <summary>
        /// Assigns an instructor to a course.
        /// </summary>
        [HttpPost("{courseId}/instructors/{instructorId}")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> AddInstructorToCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.AddInstructorToCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Removes an instructor from a course.
        /// </summary>
        [HttpDelete("{courseId}/instructors/{instructorId}")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> RemoveInstructorFromCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Gets all instructors for a course.
        /// </summary>
        [HttpGet("{courseId}/instructors")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetCourseInstructors(Guid courseId)
        {
            var instructors = await _courseService.GetCourseInstructorsAsync(courseId);
            return Ok(instructors);
        }

        /// <summary>
        /// Gets instructor assignment history for a course.
        /// </summary>
        [HttpGet("{courseId}/instructors/history")]
        [Authorize(Roles = "INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetCourseInstructorHistory(Guid courseId)
        {
            var history = await _courseService.GetCourseInstructorHistoryAsync(courseId);
            return Ok(history);
        }

        /// <summary>
        /// Gets courses taught by an instructor.
        /// </summary>
        [HttpGet("instructor/{instructorId}")]
        [Authorize(Roles = "INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetCoursesByInstructor(Guid instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorAsync(instructorId);
            return Ok(courses);
        }

        /// <summary>
        /// Gets courses enrolled by a student.
        /// </summary>
        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetCoursesByStudent(Guid studentId)
        {
            var courses = await _courseService.GetCoursesByStudentAsync(studentId);
            return Ok(courses);
        }
    }
} 