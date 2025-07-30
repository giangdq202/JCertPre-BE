using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages course operations and instructor assignments.
    /// </summary>
    [Route("api/course")]
    [ApiController]
    [Tags("Courses")]
    [Produces("application/json")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Gets courses with filtering, searching, and pagination.
        /// </summary>
        /// <param name="queryParameters">Filter, search, and pagination parameters.</param>
        /// <returns>Paginated list of courses.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] CourseQueryParameters queryParameters)
        {
            var result = await _courseService.GetCoursesWithPaginationAsync(queryParameters);
            return Ok(result);
        }

        /// <summary>
        /// Gets detailed course information.
        /// </summary>
        /// <param name="id">Course ID.</param>
        /// <returns>Complete course information.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(course);
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="createCourseDto">Course creation data.</param>
        /// <returns>Created course details.</returns>
        [HttpPost]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        /// <param name="id">Course ID.</param>
        /// <param name="updateCourseDto">Course update data.</param>
        /// <returns>Updated course details.</returns>
        [HttpPut("{id}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.UpdateCourseAsync(id, updateCourseDto);
            return Ok(course);
        }

        /// <summary>
        /// Deletes a course.
        /// </summary>
        /// <param name="id">Course ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")] // Uncomment when authentication is implemented
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates course publication status.
        /// </summary>
        /// <param name="id">Course ID.</param>
        /// <param name="status">New status (Draft, Published, Archived, Suspended).</param>
        /// <returns>No content on success.</returns>
        [HttpPatch("{id}/status")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] CourseStatus status)
        {
            await _courseService.UpdateCourseStatusAsync(id, status);
            return NoContent();
        }

        /// <summary>
        /// Assigns an instructor to a course.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <param name="instructorId">Instructor ID.</param>
        /// <returns>No content on success.</returns>
        [HttpPost("{courseId}/instructors/{instructorId}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> AddInstructorToCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.AddInstructorToCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Removes an instructor from a course.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <param name="instructorId">Instructor ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{courseId}/instructors/{instructorId}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> RemoveInstructorFromCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Gets all instructors for a course.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <returns>List of course instructors.</returns>
        [HttpGet("{courseId}/instructors")]
        public async Task<IActionResult> GetCourseInstructors(Guid courseId)
        {
            var instructors = await _courseService.GetCourseInstructorsAsync(courseId);
            return Ok(instructors);
        }

        /// <summary>
        /// Gets instructor assignment history for a course.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <returns>List of instructor history records.</returns>
        [HttpGet("{courseId}/instructors/history")]
        public async Task<IActionResult> GetCourseInstructorHistory(Guid courseId)
        {
            var history = await _courseService.GetCourseInstructorHistoryAsync(courseId);
            return Ok(history);
        }

        /// <summary>
        /// Gets courses taught by an instructor.
        /// </summary>
        /// <param name="instructorId">Instructor user ID.</param>
        /// <returns>List of courses where the user is an instructor.</returns>
        [HttpGet("instructor/{instructorId}")]
        public async Task<IActionResult> GetCoursesByInstructor(Guid instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorAsync(instructorId);
            return Ok(courses);
        }

        /// <summary>
        /// Gets courses enrolled by a student.
        /// </summary>
        /// <param name="studentId">Student user ID.</param>
        /// <returns>List of courses where the user is enrolled.</returns>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetCoursesByStudent(Guid studentId)
        {
            var courses = await _courseService.GetCoursesByStudentAsync(studentId);
            return Ok(courses);
        }
    }
} 