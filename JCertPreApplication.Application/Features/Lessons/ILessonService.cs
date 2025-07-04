using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Lessons
{
    public interface ILessonService
    {
        /// <summary>
        /// Get paginated lessons by course id and optional search by title.
        /// </summary>
        Task<Pagination<Lesson>> GetPaginatedAsync(Guid courseId, string? searchTerm, int pageIndex, int pageSize);

        /// <summary>
        /// Update lesson by lesson id.
        /// </summary>
        Task<Lesson> UpdateLessonAsync(Guid lessonId, UpdateLessonDto updateLessonDto);

        /// <summary>
        /// Create lesson by course id.
        /// </summary>
        Task<Lesson> CreateLessonAsync(Guid courseId, CreateLessonDto createLessonDto);

        /// <summary>
        /// Delete all lessons by course id.
        /// </summary>
        Task DeleteAllByCourseIdAsync(Guid courseId);

        /// <summary>
        /// Delete lesson by lesson id.
        /// </summary>
        Task DeleteLessonByIdAsync(Guid lessonId);
    }
}