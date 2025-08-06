using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Features.Tests
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ITestTemplateRepository _testTemplateRepository;
        private readonly ITestQuestionRepository _testQuestionRepository;
        private readonly ITestScoreSummaryRepository _testScoreSummaryRepository;
        private readonly ITestAttemptRepository _testAttemptRepository;
        private readonly ITestTemplateTypeRepository _testTemplateTypeRepository;

        public TestService(
            ITestRepository testRepository,
            ILessonRepository lessonRepository,
            ITestTemplateRepository testTemplateRepository,
            ITestQuestionRepository testQuestionRepository,
            ITestScoreSummaryRepository testScoreSummaryRepository,
            ITestAttemptRepository testAttemptRepository,
            ITestTemplateTypeRepository testTemplateTypeRepository)
        {
            _testRepository = testRepository;
            _lessonRepository = lessonRepository;
            _testTemplateRepository = testTemplateRepository;
            _testQuestionRepository = testQuestionRepository;
            _testScoreSummaryRepository = testScoreSummaryRepository;
            _testAttemptRepository = testAttemptRepository;
            _testTemplateTypeRepository = testTemplateTypeRepository;
        }

        /// <summary>
        /// Get all tests by user id with paging and search by title.
        /// </summary>
        public async Task<Pagination<TestDto>> GetAllByUserIdAsync(
            Guid userId,
            string? searchTerm,
            int pageIndex,
            int pageSize,
            TestType? testType = null,
            CourseLevel? courseLevel = null)
        {
            try
            {
                // Build predicate for filtering
                Expression<Func<Test, bool>> predicate = t =>
                    t.createdByUserId == userId &&
                    (string.IsNullOrEmpty(searchTerm) || t.title.ToLower().Contains(searchTerm.ToLower())) &&
                    (!testType.HasValue || t.testType == testType.Value) &&
                    (!courseLevel.HasValue || t.courseLevel == courseLevel.Value);

                // Get paginated and ordered results
                var paged = await _testRepository.GetPaginationAsync(
                    predicate,
                    "TestTemplateType",
                    pageIndex <= 0 ? 1 : pageIndex,
                    pageSize <= 0 ? 10 : (pageSize > 100 ? 100 : pageSize),
                    orderBy: q => q.OrderBy(t => t.availableFrom)
                );

                return new Pagination<TestDto>
                {
                    TotalItemsCount = paged.TotalItemsCount,
                    PageSize = paged.PageSize,
                    PageIndex = paged.PageIndex,
                    Items = paged.Items.Select(MapToTestDto).ToList()
                };
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
        public async Task<TestDto?> GetByLessonIdAsync(Guid lessonId)
        {
            try
            {
                var test = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId, "TestTemplateType");
                if (test == null)
                    throw ApiException.NotFound("Test", lessonId);

                return MapToTestDto(test);
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
        /// Get a test by test id.
        /// </summary>
        public async Task<TestDto?> GetByTestIdAsync(Guid testId)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(testId);
                if (test == null)
                    return null;
                return MapToTestDto(test);
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
        /// Handles JLPTAuto and Custom types logic for TestTemplateTypeId and durationMinutes.
        /// </summary>
        public async Task<TestDto> CreateByLessonIdAsync(Guid lessonId, CreateTestDto dto, Guid userId)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId)
                    ?? throw ApiException.NotFound("Lesson", lessonId);

                var existingTest = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId);
                if (existingTest != null)
                    throw ApiException.BadRequest("TEST_ALREADY_EXISTS", "A test already exists for this lesson. Each lesson can only have one test.");

                int durationMinutes = 0;
                Guid? testTemplateTypeId = null;
                List<TestTemplate>? templates = null;

                switch (dto.TestType)
                {
                    case TestType.JLPTAuto:
                        var jlptType = await FindActiveTemplateType(dto.CourseLevel, TestType.JLPTAuto);
                        testTemplateTypeId = jlptType.TestTemplateTypeId;
                        templates = await _testTemplateRepository.GetAllAsync(t => t.TestTemplateTypeId == testTemplateTypeId);
                        if (templates == null || templates.Count == 0)
                            throw ApiException.BadRequest("NO_TEMPLATES_FOUND", "No active test templates found for the provided TestTemplateTypeId.");
                        durationMinutes = templates.Sum(t => t.durationMinutes);
                        break;

                    case TestType.CustomManual:
                        durationMinutes = dto.DurationMinutes;
                        break;

                    case TestType.CustomAuto:
                        var customAutoType = await FindActiveTemplateType(dto.CourseLevel, TestType.JLPTAuto);
                        testTemplateTypeId = customAutoType.TestTemplateTypeId;
                        durationMinutes = 0;
                        break;
                }

                var test = new Test
                {
                    testId = Guid.NewGuid(),
                    title = dto.Title,
                    description = dto.Description,
                    testType = dto.TestType,
                    courseLevel = dto.CourseLevel,
                    durationMinutes = durationMinutes,
                    lessonId = lessonId,
                    createdByUserId = userId,
                    availableFrom = dto.AvailableFrom,
                    availableTo = dto.AvailableTo,
                    maxAttempts = dto.MaxAttempts,
                    passing_percentage = dto.PassingPercentage,
                    status = TestStatus.Close,
                    TestTemplateTypeId = testTemplateTypeId
                };

                await _testRepository.InsertAsync(test);
                await _testRepository.SaveChangesAsync();


                var createdTest = await _testRepository.GetByIdAsync(test.testId);
                if (createdTest != null && createdTest.TestTemplateTypeId.HasValue && templates != null)
                    createdTest.TestTemplateType = templates.FirstOrDefault()?.TestTemplateType;

                return MapToTestDto(createdTest ?? test);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while creating the test: {ex.Message}");
            }
        }


        /// <summary>
        /// Update a test by test id.
        /// </summary>
        public async Task<TestDto> UpdateAsync(Guid testId, UpdateTestDto dto)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(testId)
                    ?? throw ApiException.NotFound("Test", testId);

                var hasQuestions = await _testQuestionRepository.AnyAsync(q => q.testId == testId);

                if (dto.TestType.HasValue)
                {
                    if (hasQuestions)
                        throw ApiException.BadRequest("UPDATE_TEST_TYPE_NOT_ALLOWED", "Cannot update test type if there are questions.");
                    test.testType = dto.TestType.Value;
                }

                if (!string.IsNullOrWhiteSpace(dto.Title))
                    test.title = dto.Title;
                if (!string.IsNullOrWhiteSpace(dto.Description))
                    test.description = dto.Description;
                if (dto.DurationMinutes.HasValue)
                {
                    if (test.testType != TestType.CustomManual)
                        throw ApiException.BadRequest("UPDATE_DURATION_NOT_ALLOWED", "Can only update duration minutes for CustomManual test type.");
                    test.durationMinutes = dto.DurationMinutes.Value;
                }
                if (dto.AvailableFrom.HasValue)
                    test.availableFrom = dto.AvailableFrom.Value;
                if (dto.AvailableTo.HasValue)
                    test.availableTo = dto.AvailableTo.Value;
                if (dto.MaxAttempts.HasValue)
                    test.maxAttempts = dto.MaxAttempts.Value;

                if (dto.CourseLevel.HasValue)
                {
                    var newType = dto.TestType ?? test.testType;
                    if (newType == TestType.CustomManual)
                    {
                        test.courseLevel = dto.CourseLevel.Value;
                    }
                    else if (newType == TestType.JLPTAuto || newType == TestType.CustomAuto)
                    {
                        if (hasQuestions)
                            throw ApiException.BadRequest("UPDATE_COURSE_LEVEL_NOT_ALLOWED", "Cannot update course level if there are questions for JLPTAuto or CustomAuto test type.");

                        test.courseLevel = dto.CourseLevel.Value;

                        var jlptType = await FindActiveTemplateType(dto.CourseLevel.Value, TestType.JLPTAuto);
                        test.TestTemplateTypeId = jlptType.TestTemplateTypeId;
                        var templates = await _testTemplateRepository.GetAllAsync(t => t.TestTemplateTypeId == test.TestTemplateTypeId);
                        if (templates == null || templates.Count == 0)
                            throw ApiException.BadRequest("NO_TEMPLATES_FOUND", "No active test templates found for the provided TestTemplateTypeId.");
                        test.durationMinutes = templates.Sum(t => t.durationMinutes);
                    }
                }

                if (dto.PassingPercentage.HasValue)
                {
                    if (test.status != TestStatus.Close)
                        throw ApiException.BadRequest("UPDATE_PASSING_PERCENTAGE_NOT_ALLOWED", "Can only update PassingPercentage if the test is closed.");
                    test.passing_percentage = dto.PassingPercentage.Value;
                }
                await _testRepository.UpdateAsync(test);
                await _testRepository.SaveChangesAsync();
               

                var updatedTest = await _testRepository.GetFirstOrDefaultAsync(t => t.testId == testId, "TestTemplateType");
                return MapToTestDto(updatedTest ?? test);
            }
            catch (ApiException) { throw; }
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

                // Check if test is closed
                if (test.status != TestStatus.Close)
                    throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Can only delete a test if its status is closed.");

                var now = DateTime.UtcNow;

                // Efficiently check for any active test attempts
                var hasActiveAttempt = await _testAttemptRepository.AnyAsync(
                    a => a.testId == testId && now >= a.startTime && now <= a.endTime);

                if (hasActiveAttempt)
                    throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Cannot delete test while there is an active test attempt.");

                // Check if there are any test questions for this test
                var hasTestQuestions = await _testQuestionRepository.AnyAsync(q => q.testId == testId);
                if (hasTestQuestions)
                    throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Cannot delete test while there are test questions. Please remove all questions first.");

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

        /// <summary>
        /// Update the status of a test. Only allows status to be set to Open if the test has at least one question.
        /// </summary>
        public async Task<TestDto> UpdateStatusAsync(Guid testId, TestStatus status)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(testId);
                if (test == null)
                    throw ApiException.NotFound("Test", testId);

                // Check for questions only if status is being set to Open
                if (status == TestStatus.Open)
                {
                    var hasQuestions = await _testQuestionRepository.AnyAsync(q => q.testId == testId);
                    if (!hasQuestions)
                        throw ApiException.BadRequest("TEST_NO_QUESTIONS", "Cannot open test without any questions.");
                }

                test.status = status;

                await _testRepository.UpdateAsync(test);
                await _testRepository.SaveChangesAsync();

                return MapToTestDto(test);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while updating test status: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps a Test entity to a TestDto, including TestTemplateTypeId and TestTemplateTypeName if present.
        /// </summary>
        private TestDto MapToTestDto(Test test)
        {
            return new TestDto
            {
                TestId = test.testId,
                Title = test.title,
                Description = test.description,
                TestType = test.testType,
                CourseLevel = test.courseLevel,
                DurationMinutes = test.durationMinutes,
                LessonId = test.lessonId,
                CreatedByUserId = test.createdByUserId,
                AvailableFrom = test.availableFrom,
                AvailableTo = test.availableTo,
                MaxAttempts = test.maxAttempts,
                Status = test.status,
                TestTemplateTypeId = test.TestTemplateTypeId,
                TestTemplateTypeName = test.TestTemplateType != null ? test.TestTemplateType.typeName : null
            };
        }

        // Helper method for finding active TestTemplateType
        private async Task<TestTemplateType> FindActiveTemplateType(CourseLevel courseLevel, TestType testType)
        {
            var type = await _testTemplateTypeRepository.GetFirstOrDefaultAsync(
                t => t.courseLevel == courseLevel
                    && t.isActive
                    && t.testType == testType
            );
            if (type == null)
                throw ApiException.BadRequest("NO_TEMPLATE_TYPE_FOUND", $"No active test template type found for the provided course level and {testType} type.");
            return type;
        }
    }
}