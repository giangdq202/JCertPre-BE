using JCertPreApplication.Application.Dtos.LessonProgress;

namespace JCertPreApplication.Application.Features.LessonProgresses
{
    public interface ILessonProgressService
    {
        Task<List<LessonProgressDto>> GetByUserAndCourseAsync(Guid userId, Guid courseId);
        Task<LessonProgressDto?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
        Task<LessonProgressDto> CreateAsync(CreateLessonProgressDto dto);
        Task<LessonProgressDto> UpdateAsync(Guid progressId, UpdateLessonProgressDto dto);
        Task DeleteAsync(Guid progressId);
        Task<decimal> GetUserCourseCompletionRateAsync(Guid userId, Guid courseId);
    }
}