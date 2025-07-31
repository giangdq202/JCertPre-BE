using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Utilities;

namespace JCertPreApplication.Application.Features.Tests
{
    public interface ITestService
    {
        /// <summary>
        /// Get a test by test id.
        /// </summary>
        Task<TestDto?> GetByTestIdAsync(Guid testId);
        /// <summary>
        /// Get all tests by user id with paging and search by title.
        /// </summary>
        Task<Pagination<TestDto>> GetAllByUserIdAsync(Guid userId, string? searchTerm, int pageIndex, int pageSize);

        /// <summary>
        /// Get a test by lesson id.
        /// </summary>
        Task<TestDto?> GetByLessonIdAsync(Guid lessonId);

        /// <summary>
        /// Create a test by lesson id and user id.
        /// </summary>
        Task<TestDto> CreateByLessonIdAsync(Guid lessonId, CreateTestDto dto, Guid userId);

        /// <summary>
        /// Update a test by test id.
        /// </summary>
        Task<TestDto> UpdateAsync(Guid testId, UpdateTestDto dto);

        /// <summary>
        /// Update the status of a test by test id.
        /// </summary>
        Task<TestDto> UpdateStatusAsync(Guid testId, TestStatus status);

        /// <summary>
        /// Delete a test by test id.
        /// </summary>
        Task DeleteAsync(Guid testId);
    }
}