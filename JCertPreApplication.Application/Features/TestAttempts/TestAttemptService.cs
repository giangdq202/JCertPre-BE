using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.TestAttempts;
using JCertPreApplication.Domain.Entities;
using Microsoft.Extensions.Logging;

public class TestAttemptService : ITestAttemptService
{
    private readonly ITestAttemptRepository _testAttemptRepository;
    private readonly ITestRepository _testRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IAttemptAnswerRepository _attemptAnswerRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IChoiceRepository _choiceRepository;
    private readonly ITestAttemptAutoSubmitController _autoSubmitController;
    private readonly ILogger<TestAttemptService> _logger;

    public TestAttemptService(
        ITestAttemptRepository testAttemptRepository,
        ITestRepository testRepository,
        IEnrollmentRepository enrollmentRepository,
        IAttemptAnswerRepository attemptAnswerRepository,
        IQuestionRepository questionRepository,
        IChoiceRepository choiceRepository,
        ITestAttemptAutoSubmitController autoSubmitController,
        ILogger<TestAttemptService> logger)
    {
        _testAttemptRepository = testAttemptRepository;
        _testRepository = testRepository;
        _enrollmentRepository = enrollmentRepository;
        _attemptAnswerRepository = attemptAnswerRepository;
        _questionRepository = questionRepository;
        _choiceRepository = choiceRepository;
        _autoSubmitController = autoSubmitController;
        _logger = logger;
    }

    /// <summary>
    /// Start a test attempt for a user.
    /// </summary>
    public async Task<TestAttemptDto> StartTestAttemptAsync(StartTestAttemptDto dto)
    {
        try
        {
            var test = await _testRepository.GetByIdAsync(dto.TestId);
            if (test == null)
                throw ApiException.NotFound("Test", dto.TestId);

            // Check available range
            var now = DateTime.UtcNow;
            if (test.availableFrom.HasValue && now < test.availableFrom.Value)
                throw ApiException.BadRequest("TEST_NOT_AVAILABLE", "Test is not yet available.");
            if (test.availableTo.HasValue && now > test.availableTo.Value)
                throw ApiException.BadRequest("TEST_EXPIRED", "Test is no longer available.");
            if (test.status == TestStatus.Close)
                throw ApiException.BadRequest("TEST_CLOSED", "Test is closed.");
          
            // Check max attempts
            var userAttempts = await _testAttemptRepository.GetAllAsync(a => a.testId == dto.TestId && a.userId == dto.UserId);
            if (userAttempts.Count >= test.maxAttempts)
                throw ApiException.BadRequest("MAX_ATTEMPTS_REACHED", "User has reached the maximum number of attempts.");

            // If lessonId is set, check enrollment
            if (test.lessonId.HasValue)
            {
                var enrolled = await _enrollmentRepository.GetFirstOrDefaultAsync(e => e.userId == dto.UserId && e.courseId == test.lessonId.Value);
                if (enrolled == null)
                    throw ApiException.Forbidden("USER_NOT_ENROLLED", "User is not enrolled in the lesson for this test.");
            }

            // Create new attempt
            var attempt = new TestAttempt
            {
                attemptId = Guid.NewGuid(),
                userId = dto.UserId,
                testId = dto.TestId,
                startTime = now,
                endTime = now.AddMinutes(test.durationMinutes),
                attemptNumber = userAttempts.Count + 1,
                status = TestAttemptStatus.InProgress,
                totalScore = null,
                languageKnowledgeScore = test.testType == TestType.CustomManual ? null : 0,
                readingScore = test.testType == TestType.CustomManual ? null : 0,
                listeningScore = test.testType == TestType.CustomManual ? null : 0,
                isPass = test.testType == TestType.CustomManual ? null : false
            };

            await _testAttemptRepository.InsertAsync(attempt);
            await _testAttemptRepository.SaveChangesAsync();

            // Register attempt for auto-submission monitoring
            _autoSubmitController.AddAttempt(attempt.attemptId, attempt.endTime);
            _logger.LogInformation("Started test attempt {AttemptId} for user {UserId} and registered for auto-submit monitoring",
                attempt.attemptId, attempt.userId);

            return MapToDto(attempt);
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_ATTEMPT_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Submit a test attempt and calculate score.
    /// </summary>
    public async Task<TestAttemptDto> SubmitTestAttemptAsync(SubmitTestAttemptDto dto)
    {
        try
        {
            var attempt = await _testAttemptRepository.GetByIdAsync(dto.AttemptId);
            if (attempt == null)
                throw ApiException.NotFound("TestAttempt", dto.AttemptId);

            // Prevent submission if status is Suspended
            if (attempt.status == TestAttemptStatus.Suspended)
                throw ApiException.BadRequest("ATTEMPT_SUSPENDED", "Cannot submit a suspended test attempt.");

            var test = await _testRepository.GetByIdAsync(attempt.testId);
            if (test == null)
                throw ApiException.NotFound("Test", attempt.testId);

            // Calculate score for customManual
            int totalScore = 0;
            if (test.testType == TestType.CustomManual)
            {
                // Get all answers for this attempt
                var answers = await _attemptAnswerRepository.GetAllAsync(a => a.attemptId == attempt.attemptId);

                foreach (var answer in answers)
                {
                    // Get the question and choice for this answer using repositories
                    var question = await _questionRepository.GetByIdAsync(answer.questionId);
                    var choice = await _choiceRepository.GetByIdAsync(answer.choiceId);

                    // If the choice is correct, add the question's points
                    if (choice != null && choice.isCorrect && question != null)
                    {
                        totalScore += question.points;
                    }
                }

                attempt.totalScore = totalScore;
                attempt.languageKnowledgeScore = null;
                attempt.readingScore = null;
                attempt.listeningScore = null;
                attempt.isPass = null;
            }

            attempt.status = TestAttemptStatus.Completed;
            await _testAttemptRepository.UpdateAsync(attempt);
            await _testAttemptRepository.SaveChangesAsync();

            return MapToDto(attempt);
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_ATTEMPT_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Get all test attempts by user ID.
    /// </summary>
    public async Task<List<TestAttemptDto>> GetAllByUserIdAsync(Guid userId)
    {
        try
        {
            var attempts = await _testAttemptRepository.GetAllAsync(a => a.userId == userId);
            return attempts.Select(MapToDto).ToList();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("GET_ATTEMPTS_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Update the status of a test attempt.
    /// </summary>
    public async Task<TestAttemptDto> UpdateStatusAsync(Guid attemptId, TestAttemptStatus status)
    {
        try
        {
            var attempt = await _testAttemptRepository.GetByIdAsync(attemptId);
            if (attempt == null)
                throw ApiException.NotFound("TestAttempt", attemptId);

            attempt.status = status;
            await _testAttemptRepository.UpdateAsync(attempt);
            await _testAttemptRepository.SaveChangesAsync();

            _logger.LogInformation("Updated status for test attempt {AttemptId} to {Status}", attemptId, status);

            return MapToDto(attempt);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("UPDATE_ATTEMPT_STATUS_ERROR", ex.Message);
        }
    }

    // Helper: Map entity to DTO
    private static TestAttemptDto MapToDto(TestAttempt attempt)
    {
        return new TestAttemptDto
        {
            AttemptId = attempt.attemptId,
            UserId = attempt.userId,
            TestId = attempt.testId,
            AttemptNumber = attempt.attemptNumber,
            Status = attempt.status,
            StartTime = attempt.startTime,
            EndTime = attempt.endTime,
            TotalScore = attempt.totalScore,
            LanguageKnowledgeScore = attempt.languageKnowledgeScore,
            ReadingScore = attempt.readingScore,
            ListeningScore = attempt.listeningScore,
            IsPass = attempt.isPass
        };
    }
}