using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Features.Lessons
{
    /// <summary>
    /// Service for handling business logic related to Lesson entities.
    /// Implements exception handling and follows Clean Architecture best practices.
    /// </summary>
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;

        public LessonService(ILessonRepository lessonRepository, ICourseRepository courseRepository)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
        }

        /// <summary>
        /// Get paginated lessons by course id and optional search by title.
        /// </summary>
        public async Task<Pagination<Lesson>> GetPaginatedAsync(Guid courseId, string? searchTerm, int pageIndex, int pageSize)
        {
            try
            {
                // Ensure course exists
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                    throw ApiException.NotFound("Course", courseId);

                var paged = await _lessonRepository.GetPaginatedLessonsByCourseAsync(
                    courseId,
                    searchTerm,
                    pageIndex <= 0 ? 1 : pageIndex,
                    pageSize <= 0 ? 10 : (pageSize > 100 ? 100 : pageSize)
                );

                return paged;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("LESSON_SERVICE_ERROR", $"An error occurred while retrieving lessons: {ex.Message}");
            }
        }

        /// <summary>
        /// Update lesson by lesson id.
        /// </summary>
        public async Task<Lesson> UpdateLessonAsync(Guid lessonId, UpdateLessonDto updateLessonDto)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                    throw ApiException.NotFound("Lesson", lessonId);

                if (!string.IsNullOrEmpty(updateLessonDto.Title))
                    lesson.title = updateLessonDto.Title;
                if (!string.IsNullOrEmpty(updateLessonDto.Content))
                    lesson.content = updateLessonDto.Content;

                if (updateLessonDto.LessonOrder.HasValue && updateLessonDto.LessonOrder.Value != lesson.lessonOrder)
                {
                    int oldOrder = lesson.lessonOrder;
                    int newOrder = updateLessonDto.LessonOrder.Value;

                    // Count lessons in course
                    var totalLessons = await _lessonRepository.CountAsync(l => l.courseId == lesson.courseId);

                    if (newOrder < 1) newOrder = 1;
                    if (newOrder > totalLessons) newOrder = totalLessons;

                    if (newOrder < oldOrder)
                    {
                        // Shift up lessons between newOrder and oldOrder-1
                        var lessonsToShift = await _lessonRepository.GetAllAsync(
                            l => l.courseId == lesson.courseId && l.lessonOrder >= newOrder && l.lessonOrder < oldOrder
                        );
                        foreach (var l in lessonsToShift)
                        {
                            l.lessonOrder++;
                            await _lessonRepository.UpdateAsync(l);
                        }
                    }
                    else if (newOrder > oldOrder)
                    {
                        // Shift down lessons between oldOrder+1 and newOrder
                        var lessonsToShift = await _lessonRepository.GetAllAsync(
                            l => l.courseId == lesson.courseId && l.lessonOrder > oldOrder && l.lessonOrder <= newOrder
                        );
                        foreach (var l in lessonsToShift)
                        {
                            l.lessonOrder--;
                            await _lessonRepository.UpdateAsync(l);
                        }
                    }
                    lesson.lessonOrder = newOrder;
                }

                await _lessonRepository.UpdateAsync(lesson);
                await _lessonRepository.SaveChangesAsync();

                return lesson;
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("LESSON_SERVICE_ERROR", $"An error occurred while updating the lesson: {ex.Message}");
            }
        }

        /// <summary>
        /// Create lesson by course id.
        /// </summary>
        public async Task<Lesson> CreateLessonAsync(Guid courseId, CreateLessonDto createLessonDto)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                    throw ApiException.NotFound("Course", courseId);

                // Count lessons in course
                var totalLessons = await _lessonRepository.CountAsync(l => l.courseId == courseId);

                int insertOrder = createLessonDto.LessonOrder;
                if (insertOrder < 1) insertOrder = 1;
                if (insertOrder > totalLessons + 1) insertOrder = totalLessons + 1;

                // Shift lessons at or after insertOrder (batch update)
                var lessonsToShift = await _lessonRepository.GetAllAsync(
                    l => l.courseId == courseId && l.lessonOrder >= insertOrder
                );
                foreach (var l in lessonsToShift)
                {
                    l.lessonOrder++;
                    await _lessonRepository.UpdateAsync(l);
                }

                var lesson = new Lesson
                {
                    lessonId = Guid.NewGuid(),
                    courseId = courseId,
                    title = createLessonDto.Title,
                    lessonOrder = insertOrder,
                    content = createLessonDto.Content
                };

                await _lessonRepository.InsertAsync(lesson);
                await _lessonRepository.SaveChangesAsync();

                return lesson;
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("LESSON_SERVICE_ERROR", $"An error occurred while creating the lesson: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete all lessons by course id.
        /// </summary>
        public async Task DeleteAllByCourseIdAsync(Guid courseId)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(courseId);
                if (course == null)
                    throw ApiException.NotFound("Course", courseId);

                await _lessonRepository.DeleteAllByCourseIdAsync(courseId);
                await _lessonRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("LESSON_SERVICE_ERROR", $"An error occurred while deleting all lessons: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete lesson by lesson id.
        /// </summary>
        public async Task DeleteLessonByIdAsync(Guid lessonId)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                    throw ApiException.NotFound("Lesson", lessonId);

                int deletedOrder = lesson.lessonOrder;
                var courseId = lesson.courseId;

                await _lessonRepository.DeleteAsync(lesson);

                // Efficiently shift down only affected lessons
                var lessonsToShift = await _lessonRepository.GetAllAsync(
                    l => l.courseId == courseId && l.lessonOrder > deletedOrder
                );
                foreach (var l in lessonsToShift)
                {
                    l.lessonOrder--;
                    await _lessonRepository.UpdateAsync(l);
                }

                await _lessonRepository.SaveChangesAsync();
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("LESSON_SERVICE_ERROR", $"An error occurred while deleting the lesson: {ex.Message}");
            }
        }
    }
}