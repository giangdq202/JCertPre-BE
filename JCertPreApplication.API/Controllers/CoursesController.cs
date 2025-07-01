using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// 📚 Course Management API Controller
    /// </summary>
    /// <remarks>
    /// Provides comprehensive course management functionality including:
    /// - Course CRUD operations
    /// - Advanced filtering and search capabilities
    /// - Instructor management for courses
    /// - Status management and publishing workflow
    /// 
    /// **Key Features:**
    /// - Unified filtering endpoint supporting multiple criteria
    /// - Pagination for large datasets
    /// - Many-to-many instructor relationships
    /// - Course status workflow (Draft → Published → Archived/Suspended)
    /// </remarks>
    [Route("api/[controller]")]
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
        /// 🔍 Get courses with advanced filtering, searching, and pagination
        /// </summary>
        /// <remarks>
        /// **Unified endpoint** for retrieving courses with flexible filtering options.
        /// 
        /// This endpoint replaces multiple individual filtering endpoints and supports:
        /// - **Search**: Filter by title/description keywords
        /// - **Instructor Filter**: Show courses taught by specific instructor
        /// - **Status Filter**: Filter by course status (Draft, Published, etc.)
        /// - **Level Filter**: Filter by JLPT level (N5, N4, N3, N2, N1)
        /// - **Type Filter**: Filter by delivery method (Online, Offline, Hybrid)
        /// - **Pagination**: Control page size and navigate through results
        /// 
        /// **Multiple filters can be combined** in a single request for precise results.
        /// 
        /// **Examples:**
        /// ```
        /// GET /api/Courses?level=N5&amp;status=Published&amp;pageSize=20
        /// GET /api/Courses?searchTerm=beginner&amp;courseType=Online
        /// GET /api/Courses?instructorId=3fa85f64-5717-4562-b3fc-2c963f66afa6&amp;pageNumber=2
        /// ```
        /// </remarks>
        /// <param name="queryParameters">Query parameters for filtering, searching, and pagination</param>
        /// <returns>Paginated list of courses matching the specified criteria</returns>
        /// <response code="200">Successfully retrieved courses. Returns paginated results.</response>
        /// <response code="400">Invalid query parameters (e.g., invalid enum values, negative page numbers).</response>
        /// <response code="500">Internal server error occurred during processing.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Pagination<CourseListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCourses([FromQuery] CourseQueryParameters queryParameters)
        {
            var result = await _courseService.GetCoursesWithPaginationAsync(queryParameters);
            return Ok(result);
        }

        /// <summary>
        /// 📖 Get detailed course information by ID
        /// </summary>
        /// <remarks>
        /// Retrieves complete course information including:
        /// - Full course details and metadata
        /// - List of assigned instructors
        /// - Statistics (lesson count, enrollment count, etc.)
        /// - All course properties and relationships
        /// 
        /// **Use this endpoint** when you need complete course information for:
        /// - Course detail pages
        /// - Course editing forms
        /// - Administrative reviews
        /// </remarks>
        /// <param name="id">Unique course identifier (GUID format)</param>
        /// <returns>Complete course information with all related data</returns>
        /// <response code="200">Successfully retrieved course details.</response>
        /// <response code="404">Course with the specified ID was not found.</response>
        /// <response code="400">Invalid course ID format (must be a valid GUID).</response>
        /// <response code="500">Internal server error occurred during processing.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            return Ok(course);
        }

        /// <summary>
        /// ➕ Create a new course
        /// </summary>
        /// <remarks>
        /// Creates a new course with the provided information.
        /// 
        /// **Important Notes:**
        /// - Course title must be **unique** across all courses
        /// - New courses are created with **Draft** status by default
        /// - Instructors can be assigned separately using instructor management endpoints
        /// - Thumbnail URL is optional and can be added later
        /// 
        /// **Workflow:**
        /// 1. Create course (this endpoint) → Status: Draft
        /// 2. Add content and instructors
        /// 3. Update status to Published when ready
        /// 
        /// **Required Fields:**
        /// - Title (3-200 characters, must be unique)
        /// - Description (10-2000 characters)
        /// - Level (N5, N4, N3, N2, N1)
        /// - CourseType (Online, Offline, Hybrid)
        /// - Price (≥ 0, use 0 for free courses)
        /// </remarks>
        /// <param name="createCourseDto">Course creation data with all required information</param>
        /// <returns>Created course with generated ID and default values</returns>
        /// <response code="201">Course created successfully. Returns the created course data.</response>
        /// <response code="400">Validation failed. Common issues: duplicate title, invalid data, missing required fields.</response>
        /// <response code="409">Conflict. A course with the same title already exists.</response>
        /// <response code="500">Internal server error occurred during course creation.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        /// <summary>
        /// ✏️ Update an existing course
        /// </summary>
        /// <remarks>
        /// Updates an existing course with new information.
        /// 
        /// **Partial Update Support:**
        /// - Only provide the fields you want to change
        /// - Unchanged fields will retain their current values
        /// - All fields are optional, but at least one should be provided
        /// 
        /// **Special Handling:**
        /// - **Title**: Must remain unique if changed
        /// - **ThumbnailUrl**: Set to `null` to remove current thumbnail
        /// - **Status**: Can be updated here or via dedicated status endpoint
        /// 
        /// **Validation:**
        /// - Same validation rules as creation apply to updated fields
        /// - Title uniqueness is checked excluding the current course
        /// 
        /// **Use Cases:**
        /// - Content updates and corrections
        /// - Price adjustments
        /// - Status changes and publishing workflow
        /// - Thumbnail management
        /// </remarks>
        /// <param name="id">Course ID to update</param>
        /// <param name="updateCourseDto">Course update data (partial update supported)</param>
        /// <returns>Updated course with all current information</returns>
        /// <response code="200">Course updated successfully. Returns the updated course data.</response>
        /// <response code="400">Validation failed or invalid data provided.</response>
        /// <response code="404">Course with the specified ID was not found.</response>
        /// <response code="409">Conflict. Title already exists (if title was changed).</response>
        /// <response code="500">Internal server error occurred during update.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseService.UpdateCourseAsync(id, updateCourseDto);
            return Ok(course);
        }

        /// <summary>
        /// 🗑️ Delete a course
        /// </summary>
        /// <remarks>
        /// **Permanently deletes** a course from the system.
        /// 
        /// **⚠️ Important Restrictions:**
        /// - Courses with **existing enrollments cannot be deleted**
        /// - This operation is **irreversible**
        /// - Consider changing status to 'Archived' instead for courses with enrollments
        /// 
        /// **Pre-deletion Checks:**
        /// - Verifies course exists
        /// - Checks for active enrollments
        /// - Ensures data integrity
        /// 
        /// **What Gets Deleted:**
        /// - Course record and metadata
        /// - Course-instructor relationships
        /// - Associated lessons and livestreams
        /// - ⚠️ **Use with extreme caution**
        /// 
        /// **Recommended Alternative:**
        /// For courses with enrollments, use `PATCH /api/Courses/{id}/status` to set status to 'Archived' instead.
        /// </remarks>
        /// <param name="id">Course ID to delete</param>
        /// <returns>No content if deletion was successful</returns>
        /// <response code="204">Course deleted successfully.</response>
        /// <response code="400">Cannot delete course with existing enrollments.</response>
        /// <response code="404">Course with the specified ID was not found.</response>
        /// <response code="500">Internal server error occurred during deletion.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize(Roles = "Admin")] // Uncomment when authentication is implemented
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _courseService.DeleteCourseAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 🔄 Update course status
        /// </summary>
        /// <remarks>
        /// Updates the publication status of a course.
        /// 
        /// **Available Statuses:**
        /// - **Draft**: Course is being developed (default for new courses)
        /// - **Published**: Course is live and available for enrollment
        /// - **Archived**: Course is no longer active but remains accessible to enrolled students
        /// - **Suspended**: Course is temporarily unavailable (e.g., for maintenance)
        /// 
        /// **Common Workflows:**
        /// - `Draft` → `Published`: When course is ready for students
        /// - `Published` → `Archived`: When course is being retired
        /// - `Published` → `Suspended`: For temporary unavailability
        /// - `Suspended` → `Published`: To reactivate a course
        /// 
        /// **Status-specific Behavior:**
        /// - **Draft**: Only visible to instructors/admins
        /// - **Published**: Visible to all users, open for enrollment
        /// - **Archived**: No new enrollments, existing students retain access
        /// - **Suspended**: Not visible to students, enrollments paused
        /// </remarks>
        /// <param name="id">Course ID to update status for</param>
        /// <param name="status">New status to set (Draft, Published, Archived, Suspended)</param>
        /// <returns>No content if status update was successful</returns>
        /// <response code="204">Course status updated successfully.</response>
        /// <response code="400">Invalid status value provided.</response>
        /// <response code="404">Course with the specified ID was not found.</response>
        /// <response code="500">Internal server error occurred during status update.</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> UpdateCourseStatus(Guid id, [FromBody] CourseStatus status)
        {
            await _courseService.UpdateCourseStatusAsync(id, status);
            return NoContent();
        }

        /// <summary>
        /// 👨‍🏫 Add instructor to course
        /// </summary>
        /// <remarks>
        /// Assigns an instructor to a course, establishing a many-to-many relationship.
        /// 
        /// **Key Features:**
        /// - Multiple instructors can be assigned to a single course
        /// - Instructors can teach multiple courses
        /// - Duplicate assignments are automatically prevented
        /// - Validates both course and instructor existence
        /// 
        /// **Prerequisites:**
        /// - Course must exist
        /// - User (instructor) must exist in the system
        /// - User should have appropriate instructor role/permissions
        /// 
        /// **Use Cases:**
        /// - Assigning primary instructors to new courses
        /// - Adding co-instructors to existing courses
        /// - Building instructor teams for complex courses
        /// 
        /// **Note:** This operation is idempotent - adding an already assigned instructor will not cause an error.
        /// </remarks>
        /// <param name="courseId">Course ID to assign instructor to</param>
        /// <param name="instructorId">User ID of the instructor to assign</param>
        /// <returns>No content if assignment was successful</returns>
        /// <response code="204">Instructor assigned to course successfully.</response>
        /// <response code="404">Course or instructor not found.</response>
        /// <response code="400">Invalid course ID or instructor ID format.</response>
        /// <response code="500">Internal server error occurred during assignment.</response>
        [HttpPost("{courseId}/instructors/{instructorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> AddInstructorToCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.AddInstructorToCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// 👨‍🏫➖ Remove instructor from course
        /// </summary>
        /// <remarks>
        /// Removes an instructor assignment from a course.
        /// 
        /// **Important Considerations:**
        /// - Removing the last instructor from a course may affect course availability
        /// - Consider course continuity before removing instructors
        /// - Operation is safe - removing a non-assigned instructor won't cause errors
        /// 
        /// **Use Cases:**
        /// - Instructor role changes or departures
        /// - Course restructuring
        /// - Temporary instructor removal
        /// 
        /// **Best Practices:**
        /// - Ensure at least one instructor remains assigned to active courses
        /// - Communicate changes to affected students
        /// - Consider updating course status if no instructors remain
        /// 
        /// **Note:** This operation is idempotent - removing a non-assigned instructor will not cause an error.
        /// </remarks>
        /// <param name="courseId">Course ID to remove instructor from</param>
        /// <param name="instructorId">User ID of the instructor to remove</param>
        /// <returns>No content if removal was successful</returns>
        /// <response code="204">Instructor removed from course successfully.</response>
        /// <response code="404">Course not found (instructor validation is optional for removal).</response>
        /// <response code="400">Invalid course ID or instructor ID format.</response>
        /// <response code="500">Internal server error occurred during removal.</response>
        [HttpDelete("{courseId}/instructors/{instructorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        // [Authorize] // Uncomment when authentication is implemented
        public async Task<IActionResult> RemoveInstructorFromCourse(Guid courseId, Guid instructorId)
        {
            await _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId);
            return NoContent();
        }

        /// <summary>
        /// 👥 Get course instructors
        /// </summary>
        /// <remarks>
        /// Retrieves a list of all instructors assigned to a specific course.
        /// 
        /// **Returned Information:**
        /// - Instructor basic details (name, email, phone)
        /// - Unique user identifiers
        /// - Contact information for course coordination
        /// 
        /// **Use Cases:**
        /// - Displaying instructor information on course pages
        /// - Administrative course management
        /// - Student inquiries and contact
        /// - Course quality assurance
        /// 
        /// **Response Format:**
        /// Returns an array of instructor objects with essential information.
        /// Does not include sensitive data like passwords or detailed profiles.
        /// </remarks>
        /// <param name="courseId">Course ID to get instructors for</param>
        /// <returns>List of instructors assigned to the course</returns>
        /// <response code="200">Successfully retrieved course instructors. May return empty array if no instructors assigned.</response>
        /// <response code="404">Course with the specified ID was not found.</response>
        /// <response code="400">Invalid course ID format.</response>
        /// <response code="500">Internal server error occurred during retrieval.</response>
        [HttpGet("{courseId}/instructors")]
        [ProducesResponseType(typeof(IEnumerable<AppUserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCourseInstructors(Guid courseId)
        {
            var instructors = await _courseService.GetCourseInstructorsAsync(courseId);
            return Ok(instructors);
        }
    }
} 