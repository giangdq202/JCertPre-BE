using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Get courses with filtering, searching, and pagination
        /// </summary>
        /// <param name="queryParameters">Query parameters for filtering, searching, and pagination</param>
        /// <returns>Paginated list of courses</returns>
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] CourseQueryParameters queryParameters)
        {
            var result = await _courseService.GetCoursesWithPaginationAsync(queryParameters);
            return Ok(result);
        }

        /// <summary>
        /// Get course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>Course details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(course);
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="createCourseDto">Course creation data</param>
        /// <returns>Created course</returns>
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
        /// Update an existing course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="updateCourseDto">Course update data</param>
        /// <returns>Updated course</returns>
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
        /// Delete a course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")] // Uncomment when authentication is implemented
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Update course status
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="status">New status</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}/status")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] CourseStatus status)
        {
            await _courseService.UpdateCourseStatusAsync(id, status);
            return NoContent();
        }

        /// <summary>
        /// Add instructor to course
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="instructorId">Instructor user ID</param>
        /// <returns>No content</returns>
        [HttpPost("{courseId}/instructors/{instructorId}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> AddInstructorToCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.AddInstructorToCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Remove instructor from course
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="instructorId">Instructor user ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{courseId}/instructors/{instructorId}")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> RemoveInstructorFromCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// Get instructors of a course
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <returns>List of instructors</returns>
        [HttpGet("{courseId}/instructors")]
        public async Task<IActionResult> GetCourseInstructors(Guid courseId)
        {
            var instructors = await _courseService.GetCourseInstructorsAsync(courseId);
            return Ok(instructors);
        }
    }
} 