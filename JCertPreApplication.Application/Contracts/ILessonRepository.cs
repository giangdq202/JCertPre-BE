using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        // No custom paging method needed, use generic GetPaginationAsync
        Task DeleteAllByCourseIdAsync(Guid courseId);
        Task<Pagination<Lesson>> GetPaginatedLessonsByCourseAsync(
        Guid courseId,
        string? searchTerm,
        int pageIndex,
        int pageSize);
        Task<Test?> GetTestByLessonIdAsync(Guid lessonId);
        Task<bool> IsUserPassedTestAsync(Guid userId, Guid testId);
        Task<int> CountAsync(Expression<Func<Lesson, bool>> predicate);
    }
}