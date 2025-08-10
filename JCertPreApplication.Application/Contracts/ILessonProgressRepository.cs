using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILessonProgressRepository : IGenericRepository<LessonProgress>
    {
        Task<List<LessonProgress>> GetByUserAndCourseAsync(Guid userId, Guid courseId);
        Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
        Task<decimal> GetUserCourseCompletionRateAsync(Guid userId, Guid courseId);
        Task<decimal> CalculateCompletionRateAfterAddAsync(Guid userId, Guid courseId);
        Task<(int LessonOrder, decimal CompletionRate)?> GetHighestPreviousLessonProgressAsync(Guid userId, Guid courseId);
    }
}
