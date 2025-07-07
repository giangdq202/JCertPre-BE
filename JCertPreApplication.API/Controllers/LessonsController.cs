using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/lessons")]
    [ApiController]
    [Produces("application/json")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        // 4. Controller remains the same
        /// <summary>
        /// Get paginated lessons by course id and search by title.
        /// </summary>
        [HttpGet("by-course/{courseId}")]
        [ProducesResponseType(typeof(Pagination<LessonDto>), StatusCodes.Status200OK)]
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
        /// Update lesson by lesson id.
        /// </summary>
        [HttpPut("{lessonId}")]
        [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLesson(Guid lessonId, [FromBody] UpdateLessonDto updateLessonDto)
        {
            var updatedEntity = await _lessonService.UpdateLessonAsync(lessonId, updateLessonDto);
            var dto = MapToLessonDto(updatedEntity);
            return Ok(dto);
        }

        /// <summary>
        /// Create lesson by course id.
        /// </summary>
        [HttpPost("{courseId}")]
        [ProducesResponseType(typeof(LessonDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateLesson(Guid courseId, [FromBody] CreateLessonDto createLessonDto)
        {
            var createdEntity = await _lessonService.CreateLessonAsync(courseId, createLessonDto);
            var dto = MapToLessonDto(createdEntity);
            return CreatedAtAction(nameof(GetLessonsByCourseId), new { courseId = dto.CourseId }, dto);
        }

        /// <summary>
        /// Delete all lessons by course id.
        /// </summary>
        [HttpDelete("by-course/{courseId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAllByCourseId(Guid courseId)
        {
            await _lessonService.DeleteAllByCourseIdAsync(courseId);
            return NoContent();
        }

        /// <summary>
        /// Delete lesson by lesson id.
        /// </summary>
        [HttpDelete("{lessonId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteLessonById(Guid lessonId)
        {
            await _lessonService.DeleteLessonByIdAsync(lessonId);
            return NoContent();
        }

        // Mapping logic at controller layer
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