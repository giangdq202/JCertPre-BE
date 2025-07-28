using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.LessonProgress;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.LessonProgresses
{
    /// <summary>
    /// Service for handling business logic related to LessonProgress entities.
    /// </summary>
    public class LessonProgressService : ILessonProgressService
    {
        private readonly ILessonProgressRepository _repo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ILessonRepository _lessonRepo;

        public LessonProgressService(
            ILessonProgressRepository repo,
            IEnrollmentRepository enrollmentRepo,
            ILessonRepository lessonRepo)
        {
            _repo = repo;
            _enrollmentRepo = enrollmentRepo;
            _lessonRepo = lessonRepo;
        }

        /// <summary>
        /// Get all lesson progress records for a user in a course.
        /// </summary>
        public async Task<List<LessonProgressDto>> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            try
            {
                var progresses = await _repo.GetByUserAndCourseAsync(userId, courseId);
                return progresses.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_LESSON_PROGRESS_BY_USER_COURSE_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Get a lesson progress record by user and lesson.
        /// </summary>
        public async Task<LessonProgressDto?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
        {
            try
            {
                var progress = await _repo.GetByUserAndLessonAsync(userId, lessonId);
                return progress == null ? null : MapToDto(progress);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_LESSON_PROGRESS_BY_USER_LESSON_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Create a new lesson progress record.
        /// </summary>
        public async Task<LessonProgressDto> CreateAsync(CreateLessonProgressDto dto)
        {
            try
            {
                var existing = await _repo.GetByUserAndLessonAsync(dto.UserId, dto.LessonId);
                if (existing != null)
                    throw ApiException.BadRequest("LESSON_PROGRESS_EXISTS", "User already has a progress record for this lesson.");

                // Use generic repo for lesson lookup
                var lesson = await _lessonRepo.GetByIdAsync(dto.LessonId);
                if (lesson == null)
                    throw ApiException.NotFound("Lesson", dto.LessonId);

                // Check enrollment using enrollment repo
                var isEnrolled = await _enrollmentRepo.IsUserEnrolledInCourseAsync(dto.UserId, lesson.courseId);
                if (!isEnrolled)
                    throw ApiException.BadRequest("USER_NOT_ENROLLED", "User is not enrolled in the course for this lesson.");

                // Get lessonIds for the course
                var lessonIds = (await _lessonRepo.GetAllAsync(l => l.courseId == dto.CourseId)).Select(l => l.lessonId).ToList();

                var userProgresses = await _repo.GetAllAsync(lp => lp.userId == dto.UserId && lessonIds.Contains(lp.lessonId));
                var userProgressesCount = userProgresses.Count;
                // Calculate completion rate as if the new progress is already added
                var completionRate = lessonIds.Count == 0 ? 0.0m :
                    Math.Round((decimal)(userProgressesCount + 1) / lessonIds.Count * 100, 2);

                var entity = new LessonProgress
                {
                    progressId = Guid.NewGuid(),
                    userId = dto.UserId,
                    lessonId = dto.LessonId,
                    completionRate = completionRate
                };
                await _repo.InsertAsync(entity);
                await _repo.SaveChangesAsync();

                var created = await _repo.GetByIdAsync(entity.progressId);
                return MapToDto(created!);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CREATE_LESSON_PROGRESS_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Update a lesson progress record.
        /// </summary>
        public async Task<LessonProgressDto> UpdateAsync(Guid progressId, UpdateLessonProgressDto dto)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(progressId);
                if (entity == null)
                    throw ApiException.NotFound("LessonProgress", progressId);

                entity.completionRate = dto.CompletionRate;

                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                var updated = await _repo.GetByIdAsync(progressId);
                return MapToDto(updated!);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("UPDATE_LESSON_PROGRESS_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Delete a lesson progress record.
        /// </summary>
        public async Task DeleteAsync(Guid progressId)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(progressId);
                if (entity == null)
                    throw ApiException.NotFound("LessonProgress", progressId);

                await _repo.DeleteAsync(entity);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("DELETE_LESSON_PROGRESS_ERROR", ex.Message);
            }
        }

        /// <summary>
        /// Get the current user's overall completion rate for a course.
        /// </summary>
        public async Task<decimal> GetUserCourseCompletionRateAsync(Guid userId, Guid courseId)
        {
            try
            {
                return await _repo.GetUserCourseCompletionRateAsync(userId, courseId);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_USER_COURSE_COMPLETION_RATE_ERROR", ex.Message);
            }
        }

        private static LessonProgressDto MapToDto(LessonProgress entity)
        {
            return new LessonProgressDto
            {
                ProgressId = entity.progressId,
                UserId = entity.userId,
                LessonId = entity.lessonId,
                CompletionRate = entity.completionRate,
                UserFullName = entity.User?.fullName,
                LessonTitle = entity.Lesson?.title
            };
        }
    }
}