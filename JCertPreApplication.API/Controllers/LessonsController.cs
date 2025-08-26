using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages lesson operations including creation, updates, and deletion.
    /// </summary>
    [Route("api/lessons")]
    [ApiController]
    [Tags("Lessons")]
    [Produces("application/json")]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService ?? throw new ArgumentNullException(nameof(lessonService));
        }

        /// <summary>
        /// Gets paginated lessons for a course.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <param name="searchTerm">Optional title search term.</param>
        /// <param name="pageIndex">Page number (starts from 1).</param>
        /// <param name="pageSize">Items per page.</param>
        /// <returns>Paginated list of lessons.</returns>
        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetLessonsByCourseId(
            Guid courseId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagedEntities = await _lessonService.GetPaginatedAsync(courseId, searchTerm, pageIndex, pageSize);
            var pagedDtos = new Pagination<LessonDto>
            {
                TotalItemsCount = pagedEntities.TotalItemsCount,
                PageSize = pagedEntities.PageSize,
                PageIndex = pagedEntities.PageIndex,
                Items = pagedEntities.Items.Select(MapToLessonDto).ToList()
            };
            return Ok(pagedDtos);
        }

        /// <summary>
        /// Updates a lesson.
        /// </summary>
        /// <param name="lessonId">Lesson ID.</param>
        /// <param name="updateLessonDto">Updated lesson data.</param>
        /// <returns>Updated lesson details.</returns>
        [HttpPut("{lessonId}")]
        public async Task<IActionResult> UpdateLesson(Guid lessonId, [FromBody] UpdateLessonDto updateLessonDto)
        {
            var updatedEntity = await _lessonService.UpdateLessonAsync(lessonId, updateLessonDto);
            var dto = MapToLessonDto(updatedEntity);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a new lesson.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <param name="createLessonDto">Lesson creation data.</param>
        /// <returns>Created lesson details.</returns>
        [HttpPost("{courseId}")]
        public async Task<IActionResult> CreateLesson(Guid courseId, [FromBody] CreateLessonDto createLessonDto)
        {
            var createdEntity = await _lessonService.CreateLessonAsync(courseId, createLessonDto);
            var dto = MapToLessonDto(createdEntity);
            return CreatedAtAction(nameof(GetLessonsByCourseId), new { courseId = dto.CourseId }, dto);
        }

        /// <summary>
        /// Deletes all lessons in a course.
        /// </summary>
        /// <param name="courseId">Course ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("by-course/{courseId}")]
        public async Task<IActionResult> DeleteAllByCourseId(Guid courseId)
        {
            await _lessonService.DeleteAllByCourseIdAsync(courseId);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific lesson.
        /// </summary>
        /// <param name="lessonId">Lesson ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{lessonId}")]
        public async Task<IActionResult> DeleteLessonById(Guid lessonId)
        {
            await _lessonService.DeleteLessonByIdAsync(lessonId);
            return NoContent();
        }

        private static LessonDto MapToLessonDto(Lesson lesson)
        {
            return new LessonDto
            {
                LessonId = lesson.lessonId,
                CourseId = lesson.courseId,
                Title = lesson.title,
                LessonOrder = lesson.lessonOrder,
                Content = lesson.content
            };
        }
    }
}