using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        // No custom paging method needed, use generic GetPaginationAsync
        Task DeleteAllByCourseIdAsync(Guid courseId);
    }
}