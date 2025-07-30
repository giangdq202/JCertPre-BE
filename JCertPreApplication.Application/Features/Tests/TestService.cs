using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Features.Tests
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly ITestTemplateRepository _testTemplateRepository;
        private readonly ITestQuestionRepository _testQuestionRepository;
        private readonly ITestScoreSummaryRepository _testScoreSummaryRepository; // Inject this

        public TestService(
            ITestRepository testRepository,
            ILessonRepository lessonRepository,
            ITestTemplateRepository testTemplateRepository,
            ITestQuestionRepository testQuestionRepository,
            ITestScoreSummaryRepository testScoreSummaryRepository) // Add to constructor
        {
            _testRepository = testRepository;
            _lessonRepository = lessonRepository;
            _testTemplateRepository = testTemplateRepository;
            _testQuestionRepository = testQuestionRepository;
            _testScoreSummaryRepository = testScoreSummaryRepository;
        }

        /// <summary>
        /// Get all tests by user id with paging and search by title.
        /// </summary>
        public async Task<Pagination<TestDto>> GetAllByUserIdAsync(Guid userId, string? searchTerm, int pageIndex, int pageSize)
        {
            try
            {
                Expression<Func<Test, bool>> predicate = t =>
                    t.createdByUserId == userId &&
                    (string.IsNullOrEmpty(searchTerm) || t.title.ToLower().Contains(searchTerm.ToLower()));

                var paged = await _testRepository.GetPaginationAsync(
                    predicate,
                    "TestTemplateType",
                    pageIndex <= 0 ? 1 : pageIndex,
                    pageSize <= 0 ? 10 : (pageSize > 100 ? 100 : pageSize)
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
                // Ensure lesson exists
                var lesson = await _lessonRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                    throw ApiException.NotFound("Lesson", lessonId);

                // Ensure no test already exists for this lesson
                var existingTest = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId);
                if (existingTest != null)
                    throw ApiException.BadRequest("TEST_ALREADY_EXISTS", "A test already exists for this lesson. Each lesson can only have one test.");

                int durationMinutes = 0;
                Guid? testTemplateTypeId = null;
                List<TestTemplate>? templates = null;

                if (dto.TestType == TestType.JLPTAuto)
                {
                    // JLPTAuto must have a TestTemplateTypeId
                    if (!dto.TestTemplateTypeId.HasValue)
                        throw ApiException.BadRequest("TEST_TEMPLATE_TYPE_ID_REQUIRED", "TestTemplateTypeId is required for JLPTAuto test type.");

                    testTemplateTypeId = dto.TestTemplateTypeId.Value;

                    // Get all TestTemplates for this type and sum their durationMinutes
                    templates = await _testTemplateRepository.GetAllAsync(t => t.TestTemplateTypeId == testTemplateTypeId);
                    if (templates == null || templates.Count == 0)
                        throw ApiException.BadRequest("NO_TEMPLATES_FOUND", "No test templates found for the provided TestTemplateTypeId.");

                    durationMinutes = templates.Sum(t => t.durationMinutes);
                }
                else if (dto.TestType == TestType.CustomManual)
                {
                    // Only CustomManual allows user to input durationMinutes
                    durationMinutes = dto.DurationMinutes;
                    testTemplateTypeId = null;
                }
                else if (dto.TestType == TestType.CustomAuto)
                {
                    // CustomAuto: durationMinutes is set to zero or a default value
                    durationMinutes = 0;
                    testTemplateTypeId = null;
                }

                var test = new Test
                {
                    testId = Guid.NewGuid(),
                    title = dto.Title,
                    description = dto.Description,
                    testType = dto.TestType,
                    durationMinutes = durationMinutes,
                    lessonId = lessonId,
                    createdByUserId = userId,
                    availableFrom = dto.AvailableFrom,
                    availableTo = dto.AvailableTo,
                    maxAttempts = dto.MaxAttempts,
                    status = TestStatus.Close, // Always set default status
                    TestTemplateTypeId = testTemplateTypeId
                };
                // If TestTemplateTypeId is set, it will be used to determine the type name later
                await _testRepository.InsertAsync(test);
                await _testRepository.SaveChangesAsync();

                // Create test score summary
                await CreateTestScoreSummaryAsync(test.testId, dto.PassingPercentage);

                // Reload with TestTemplateType for type name
                var createdTest = await _testRepository.GetByIdAsync(test.testId);
                if (createdTest != null && createdTest.TestTemplateTypeId.HasValue && templates != null)
                {
                    createdTest.TestTemplateType = templates.FirstOrDefault()?.TestTemplateType;
                }

                return MapToTestDto(createdTest ?? test);
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

        private async Task CreateTestScoreSummaryAsync(Guid testId, decimal passingPercentage)
        {
            var testScoreSummary = new TestScoreSummary
            {
                TestScoreSummaryId = Guid.NewGuid(),
                TestId = testId,
                TestAttemptId = null,
                kanji_score = 0,
                kanji_max_score = 0,
                vocab_score = 0,
                vocab_max_score = 0,
                grammar_score = 0,
                grammar_max_score = 0,
                reading_score = 0,
                reading_max_score = 0,
                listening_score = 0,
                listening_max_score = 0,
                total_score = 0,
                percentage_score = 0,
                passing_percentage = passingPercentage
            };

            await _testScoreSummaryRepository.InsertAsync(testScoreSummary);
            await _testScoreSummaryRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Update a test by test id.
        /// </summary>
        public async Task<TestDto> UpdateAsync(Guid testId, UpdateTestDto dto)
        {
            try
            {
                var test = await _testRepository.GetByIdAsync(testId);
                if (test == null)
                    throw ApiException.NotFound("Test", testId);

                // Check if test has any questions
                var hasQuestions = await _testQuestionRepository.AnyAsync(q => q.testId == testId);

                // Do not allow test type update if there are questions
                if (dto.TestType.HasValue)
                {
                    if (hasQuestions)
                        throw ApiException.BadRequest("UPDATE_TEST_TYPE_NOT_ALLOWED", "Cannot update test type if there are questions.");
                    test.testType = dto.TestType.Value;
                }

                // Only allow durationMinutes update if test type is CustomManual
                if (dto.DurationMinutes.HasValue)
                {
                    if (test.testType != TestType.CustomManual)
                        throw ApiException.BadRequest("UPDATE_DURATION_NOT_ALLOWED", "Can only update duration minutes for CustomManual test type.");
                    test.durationMinutes = dto.DurationMinutes.Value;
                }

                // Only allow TestTemplateTypeId update if no questions and test type is JLPTAuto
                if (dto.TestTemplateTypeId.HasValue)
                {
                    if (hasQuestions)
                        throw ApiException.BadRequest("UPDATE_TEMPLATE_TYPE_ID_NOT_ALLOWED", "Cannot update TestTemplateTypeId if there are questions.");
                    if (test.testType != TestType.JLPTAuto)
                        throw ApiException.BadRequest("UPDATE_TEMPLATE_TYPE_ID_NOT_ALLOWED", "Can only update TestTemplateTypeId for JLPTAuto test type.");
                    test.TestTemplateTypeId = dto.TestTemplateTypeId.Value;
                }

                // Only allow PassingPercentage update if test is closed
                if (dto.PassingPercentage.HasValue)
                {
                    if (test.status != TestStatus.Close)
                        throw ApiException.BadRequest("UPDATE_PASSING_PERCENTAGE_NOT_ALLOWED", "Can only update PassingPercentage if the test is closed.");

                    var summary = await _testScoreSummaryRepository.GetFirstOrDefaultAsync(
                        s => s.TestId == testId && s.TestAttemptId == null);
                    if (summary != null)
                    {
                        summary.passing_percentage = dto.PassingPercentage.Value;
                        await _testScoreSummaryRepository.UpdateAsync(summary);
                    }
                }

                await _testRepository.UpdateAsync(test);
                await _testRepository.SaveChangesAsync();
                await _testScoreSummaryRepository.SaveChangesAsync();

                // Reload with TestTemplateType for type name
                var updatedTest = await _testRepository.GetFirstOrDefaultAsync(t => t.testId == testId, "TestTemplateType");

                return MapToTestDto(updatedTest ?? test);
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
    }
}