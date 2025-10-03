using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.TestQuestions;
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
        private readonly ITestAttemptRepository _testAttemptRepository;
        private readonly ITestTemplateTypeRepository _testTemplateTypeRepository;
        private readonly ITestQuestionService _testQuestionService;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentProfileRepository _studentProfileRepository;

        public TestService(
            ITestRepository testRepository,
            ILessonRepository lessonRepository,
            ITestTemplateRepository testTemplateRepository,
            ITestQuestionRepository testQuestionRepository,
            ITestAttemptRepository testAttemptRepository,
            ITestTemplateTypeRepository testTemplateTypeRepository,
            ITestQuestionService testQuestionService,
            IEnrollmentRepository enrollmentRepository,
            IStudentProfileRepository studentProfileRepository)
        {
            _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
            _lessonRepository = lessonRepository ?? throw new ArgumentNullException(nameof(lessonRepository));
            _testTemplateRepository = testTemplateRepository ?? throw new ArgumentNullException(nameof(testTemplateRepository));
            _testQuestionRepository = testQuestionRepository ?? throw new ArgumentNullException(nameof(testQuestionRepository));
            _testAttemptRepository = testAttemptRepository ?? throw new ArgumentNullException(nameof(testAttemptRepository));
            _testTemplateTypeRepository = testTemplateTypeRepository ?? throw new ArgumentNullException(nameof(testTemplateTypeRepository));
            _testQuestionService = testQuestionService ?? throw new ArgumentNullException(nameof(testQuestionService));
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
            _studentProfileRepository = studentProfileRepository ?? throw new ArgumentNullException(nameof(studentProfileRepository));
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
                Expression<Func<Test, bool>> predicate = t =>
                    t.createdByUserId == userId &&
                    (string.IsNullOrEmpty(searchTerm) || t.title.ToLower().Contains(searchTerm.ToLower())) &&
                    (!testType.HasValue || t.testType == testType.Value) &&
                    (!courseLevel.HasValue || t.courseLevel == courseLevel.Value);

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
            catch (ApiException) { throw; }
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
            catch (ApiException) { throw; }
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
            catch (ApiException) { throw; }
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

                //var existingTest = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId);
                //if (existingTest != null)
                //    throw ApiException.BadRequest("TEST_ALREADY_EXISTS", "A test already exists for this lesson. Each lesson can only have one test.");

                var test = new Test
                {
                    testId = Guid.NewGuid(),
                    title = dto.Title,
                    description = dto.Description,
                    testType = TestType.CustomManual,
                    courseLevel = dto.CourseLevel,
                    durationMinutes = dto.DurationMinutes,
                    lessonId = lessonId,
                    createdByUserId = userId,
                    availableFrom = dto.AvailableFrom,
                    availableTo = dto.AvailableTo,
                    maxAttempts = dto.MaxAttempts,
                    passing_percentage = dto.PassingPercentage,
                    status = TestStatus.Close,
                    TestTemplateTypeId = null
                };

                await _testRepository.InsertAsync(test);
                await _testRepository.SaveChangesAsync();

                var createdTest = await _testRepository.GetByIdAsync(test.testId);

                return MapToTestDto(createdTest ?? test);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while creating the test: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a writing test by lesson id and user id. Each lesson can only have one test.
        /// TestType will be WrittenManual.
        /// </summary>
        public async Task<TestDto> CreateWritingByLessonIdAsync(Guid lessonId, CreateTestDto dto, Guid userId)
        {
            try
            {
                var lesson = await _lessonRepository.GetByIdAsync(lessonId)
                    ?? throw ApiException.NotFound("Lesson", lessonId);

                //var existingTest = await _testRepository.GetFirstOrDefaultAsync(t => t.lessonId == lessonId);
                //if (existingTest != null)
                //    throw ApiException.BadRequest("TEST_ALREADY_EXISTS", "A test already exists for this lesson. Each lesson can only have one test.");

                var test = new Test
                {
                    testId = Guid.NewGuid(),
                    title = dto.Title,
                    description = dto.Description,
                    testType = TestType.WrittenManual, // Set to writing test type
                    courseLevel = dto.CourseLevel,
                    durationMinutes = dto.DurationMinutes,
                    lessonId = lessonId,
                    createdByUserId = userId,
                    availableFrom = dto.AvailableFrom,
                    availableTo = dto.AvailableTo,
                    maxAttempts = dto.MaxAttempts,
                    passing_percentage = dto.PassingPercentage,
                    status = TestStatus.Close,
                    TestTemplateTypeId = null
                };

                await _testRepository.InsertAsync(test);
                await _testRepository.SaveChangesAsync();

                var createdTest = await _testRepository.GetByIdAsync(test.testId);

                return MapToTestDto(createdTest ?? test);
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while creating the writing test: {ex.Message}");
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

                if (!string.IsNullOrWhiteSpace(dto.Title))
                    test.title = dto.Title;
                if (!string.IsNullOrWhiteSpace(dto.Description))
                    test.description = dto.Description;
                if (dto.DurationMinutes.HasValue)
                    test.durationMinutes = dto.DurationMinutes.Value;
                if (dto.AvailableFrom.HasValue)
                    test.availableFrom = dto.AvailableFrom.Value;
                if (dto.AvailableTo.HasValue)
                    test.availableTo = dto.AvailableTo.Value;
                if (dto.MaxAttempts.HasValue)
                    test.maxAttempts = dto.MaxAttempts.Value;
                if (dto.CourseLevel.HasValue)
                   test.courseLevel = dto.CourseLevel.Value;
                if (dto.PassingPercentage.HasValue)
                {
                    if (test.status != TestStatus.Close)
                        throw ApiException.BadRequest("UPDATE_PASSING_PERCENTAGE_NOT_ALLOWED", "Can only update PassingPercentage if the test is closed.");
                    test.passing_percentage = dto.PassingPercentage.Value;
                }
                await _testRepository.UpdateAsync(test);
                await _testRepository.SaveChangesAsync();

                var updatedTest = await _testRepository.GetFirstOrDefaultAsync(t => t.testId == testId);
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
            catch (ApiException) { throw; }
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
            catch (ApiException) { throw; }
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
            try
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
                    PassingPercentage = test.passing_percentage, // <-- Added mapping
                    Status = test.status,
                    TestTemplateTypeId = test.TestTemplateTypeId,
                    TestTemplateTypeName = test.TestTemplateType != null ? test.TestTemplateType.typeName : null
                };
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while mapping test to DTO: {ex.Message}");
            }
        }

        public async Task<CreateAutoTestResult> CreateAutoTestAndAddQuestionsAsync(CreateAutoTestInput input, Guid userId)
        {
            try
            {
                // 1. Check enrollment
                if (!await _enrollmentRepository.IsUserEnrolledInAnyCourseAsync(userId))
                    throw ApiException.BadRequest("USER_NOT_ENROLLED", "User must be enrolled in at least one course to take an auto test.");

                // 2. Check student profile and per-day test limit
                var studentProfile = await _studentProfileRepository.GetFirstOrDefaultAsync(sp => sp.userId == userId);
                if (studentProfile == null)
                    throw ApiException.BadRequest("NO_STUDENT_PROFILE", "Student profile not found.");

                var now = DateTime.UtcNow;

                // If lastResetTestTime is not today, reset the counter
                if (studentProfile.lastResetTestTime == null || studentProfile.lastResetTestTime.Value.Date < now.Date)
                {
                    studentProfile.numberOfTestsTaken = 0;
                }

                if (studentProfile.numberOfTestsTaken >= 10)
                    throw ApiException.BadRequest("TEST_LIMIT_REACHED", "You can only take the auto test 10 times per day.");

                // Increment the counter and update the last reset time
                studentProfile.numberOfTestsTaken += 1;
                studentProfile.lastResetTestTime = now;
                await _studentProfileRepository.UpdateAsync(studentProfile);
                await _studentProfileRepository.SaveChangesAsync();


                // 4. Continue with the original logic
                var templateType = await _testTemplateTypeRepository.GetFirstOrDefaultAsync(
                    t => t.courseLevel == input.CourseLevel
                        && t.isActive
                        && t.testType == input.TestType
                );
                if (templateType == null)
                    throw ApiException.BadRequest("NO_TEMPLATE_TYPE_FOUND", "No active test template type found for the provided course level and test type.");

                var templates = await _testTemplateRepository.GetAllAsync(t => t.TestTemplateTypeId == templateType.TestTemplateTypeId);
                if (templates == null || templates.Count == 0)
                    throw ApiException.BadRequest("NO_TEMPLATES_FOUND", "No test templates found for the provided template type.");

                int durationMinutes = templates.Sum(t => t.durationMinutes);

                var test = new Test
                {
                    testId = Guid.NewGuid(),
                    title = $"{templateType.typeName} {input.CourseLevel} Auto Test",
                    description = $"Auto-generated test for {templateType.typeName} {input.CourseLevel}",
                    testType = input.TestType,
                    courseLevel = input.CourseLevel,
                    durationMinutes = durationMinutes,
                    lessonId = null,
                    createdByUserId = userId,
                    availableFrom = now,
                    availableTo = now.AddYears(5),
                    maxAttempts = 1,
                    passing_percentage = templateType.totalPassPercentage,
                    status = TestStatus.Close,
                    TestTemplateTypeId = templateType.TestTemplateTypeId
                };

                await _testRepository.InsertAsync(test);
                await _testRepository.SaveChangesAsync();

                await _testQuestionService.AddQuestionsJLPTAutoAsync(test.testId);

                test.status = TestStatus.Open;
                await _testRepository.UpdateAsync(test);
                await _testRepository.SaveChangesAsync();

                return new CreateAutoTestResult
                {
                    TestId = test.testId,
                    Title = test.title,
                    Description = test.description,
                    DurationMinutes = test.durationMinutes,
                    TestTemplateTypeId = test.TestTemplateTypeId ?? Guid.Empty,
                    PassingPercentage = test.passing_percentage,
                    Status = test.status
                };
            }
            catch (ApiException) { throw; }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_SERVICE_ERROR", $"An error occurred while creating auto test and adding questions: {ex.Message}");
            }
        }
    }
}