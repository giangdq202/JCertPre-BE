using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Features.Tests
{
    /// <summary>
    /// Service for handling business logic related to Test entities.
    /// Implements exception handling and follows Clean Architecture best practices.
    /// </summary>
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ILessonRepository _lessonRepository;

        public TestService(ITestRepository testRepository, ILessonRepository lessonRepository)
        {
            _testRepository = testRepository;
            _lessonRepository = lessonRepository;
        }

        /// <summary>
        /// Get all tests by user id with paging and search by title.
        /// </summary>
        public async Task<Pagination<Test>> GetAllByUserIdAsync(Guid userId, string? searchTerm, int pageIndex, int pageSize)
        {
            try
            {
                Expression<Func<Test, bool>> predicate = t =>
                    t.createdByUserId == userId &&
                    (string.IsNullOrEmpty(searchTerm) || t.title.ToLower().Contains(searchTerm.ToLower()));

                var paged = await _testRepository.GetPaginationAsync(
                    predicate,
                    null,
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
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while retrieving tests: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a test by lesson id.
        /// </summary>
        public async Task<Test?> GetByLessonIdAsync(Guid lessonId)
        {
            try
            {
                var test = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId);
                if (test == null)
                    throw ApiException.NotFound("Test", lessonId);
                return test;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while retrieving the test: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a test by lesson id and user id. Each lesson can only have one test.
        /// </summary>
        public async Task<Test> CreateByLessonIdAsync(Guid lessonId, CreateTestDto dto, Guid userId)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                    throw ApiException.NotFound("Lesson", lessonId);

                // Check if a test already exists for this lesson
                var existingTest = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId);
                if (existingTest != null)
                    throw ApiException.BadRequest("TEST_ALREADY_EXISTS", "A test already exists for this lesson. Each lesson can only have one test.");

                var test = new Test
                {
                    testId = Guid.NewGuid(),
                    title = dto.Title,
                    description = dto.Description,
                    testType = dto.TestType,
                    durationMinutes = dto.DurationMinutes,
                    lessonId = lessonId,
                    createdByUserId = userId
                };

                await _testRepository.InsertAsync(test);
                await _testRepository.SaveChangesAsync();

                return test;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while creating the test: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a test by test id.
        /// </summary>
        public async Task<Test> UpdateAsync(Guid testId, UpdateTestDto dto)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(testId);
                if (test == null)
                    throw ApiException.NotFound("Test", testId);

                if (!string.IsNullOrEmpty(dto.Title))
                    test.title = dto.Title;
                if (!string.IsNullOrEmpty(dto.Description))
                    test.description = dto.Description;
                if (!string.IsNullOrEmpty(dto.TestType))
                    test.testType = dto.TestType;
                if (dto.DurationMinutes.HasValue)
                    test.durationMinutes = dto.DurationMinutes.Value;

                await _testRepository.UpdateAsync(test);
                await _testRepository.SaveChangesAsync();

                return test;
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while updating the test: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a test by test id.
        /// </summary>
        public async Task DeleteAsync(Guid testId)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(testId);
                if (test == null)
                    throw ApiException.NotFound("Test", testId);

                await _testRepository.DeleteAsync(test);
                await _testRepository.SaveChangesAsync();
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while deleting the test: {ex.Message}");
            }
        }
    }
}