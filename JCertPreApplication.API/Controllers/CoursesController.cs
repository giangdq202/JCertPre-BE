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
        /// Get all courses with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for title and description</param>
        /// <returns>Paginated list of courses</returns>
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var result = await _courseService.GetCoursesWithPaginationAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }

        /// <summary>
        /// Get all courses without pagination
        /// </summary>
        /// <returns>List of all courses</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
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

            // For now, use a hardcoded user ID. In real implementation, get from JWT token
            // var staffUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
            var staffUserId = Guid.NewGuid(); // Temporary - replace with actual user ID from authentication

            var course = await _courseService.CreateCourseAsync(createCourseDto, staffUserId);
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
        /// Get courses by instructor/staff user ID
        /// </summary>
        /// <param name="instructorId">Instructor user ID</param>
        /// <returns>List of courses created by the instructor</returns>
        [HttpGet("instructor/{instructorId}")]
        public async Task<IActionResult> GetCoursesByInstructor(Guid instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorAsync(instructorId);
            return Ok(courses);
        }

        /// <summary>
        /// Get courses by status
        /// </summary>
        /// <param name="status">Course status (Draft, Published, Archived, Suspended)</param>
        /// <returns>List of courses with the specified status</returns>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetCoursesByStatus(string status)
        {
            var courses = await _courseService.GetCoursesByStatusAsync(status);
            return Ok(courses);
        }

        /// <summary>
        /// Get courses by level
        /// </summary>
        /// <param name="level">Course level (Beginner, Intermediate, Advanced)</param>
        /// <returns>List of courses with the specified level</returns>
        [HttpGet("level/{level}")]
        public async Task<IActionResult> GetCoursesByLevel(CourseLevel level)
        {
            var courses = await _courseService.GetCoursesByLevelAsync(level);
            return Ok(courses);
        }

        /// <summary>
        /// Get courses by type
        /// </summary>
        /// <param name="type">Course type</param>
        /// <returns>List of courses with the specified type</returns>
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetCoursesByType(CourseType type)
        {
            var courses = await _courseService.GetCoursesByTypeAsync(type);
            return Ok(courses);
        }

        /// <summary>
        /// Update course status
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="status">New status</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}/status")]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] string status)
        {
            await _courseService.UpdateCourseStatusAsync(id, status);
            return NoContent();
        }
    }
} 