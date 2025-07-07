using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Tests
{
    public interface ITestService
    {
        /// <summary>
        /// Get all tests by user id with paging and search by title.
        /// </summary>
        Task<Pagination<Test>> GetAllByUserIdAsync(Guid userId, string? searchTerm, int pageIndex, int pageSize);

        /// <summary>
        /// Get a test by lesson id.
        /// </summary>
        Task<Test?> GetByLessonIdAsync(Guid lessonId);

        /// <summary>
        /// Create a test by lesson id and user id.
        /// </summary>
        Task<Test> CreateByLessonIdAsync(Guid lessonId, CreateTestDto dto, Guid userId);

        /// <summary>
        /// Update a test by test id.
        /// </summary>
        Task<Test> UpdateAsync(Guid testId, UpdateTestDto dto);

        /// <summary>
        /// Delete a test by test id.
        /// </summary>
        Task DeleteAsync(Guid testId);
    }
}