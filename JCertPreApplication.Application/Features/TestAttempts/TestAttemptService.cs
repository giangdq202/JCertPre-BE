using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.TestAttempts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
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
    private readonly ITestScoreSummaryRepository _testScoreSummaryRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly ITestTemplateRepository _testTemplateRepository;
    private readonly ITestTemplateConfigRepository _testTemplateConfigRepository;
    private readonly ISubContentRepository _subContentRepository;
    private readonly ITestQuestionRepository _testQuestionRepository;
    private readonly ITestTemplateTypeRepository _testTemplateTypeRepository;

    public TestAttemptService(
        ITestAttemptRepository testAttemptRepository,
        ITestRepository testRepository,
        IEnrollmentRepository enrollmentRepository,
        IAttemptAnswerRepository attemptAnswerRepository,
        IQuestionRepository questionRepository,
        IChoiceRepository choiceRepository,
        ITestAttemptAutoSubmitController autoSubmitController,
        ILogger<TestAttemptService> logger,
        ITestScoreSummaryRepository testScoreSummaryRepository,
        ILessonRepository lessonRepository,
        ITestTemplateRepository testTemplateRepository,
        ITestTemplateConfigRepository testTemplateConfigRepository,
        ISubContentRepository subContentRepository,
        ITestQuestionRepository testQuestionRepository,
        ITestTemplateTypeRepository testTemplateTypeRepository)
    {
        _testAttemptRepository = testAttemptRepository;
        _testRepository = testRepository;
        _enrollmentRepository = enrollmentRepository;
        _attemptAnswerRepository = attemptAnswerRepository;
        _questionRepository = questionRepository;
        _choiceRepository = choiceRepository;
        _autoSubmitController = autoSubmitController;
        _logger = logger;
        _testScoreSummaryRepository = testScoreSummaryRepository;
        _lessonRepository = lessonRepository;
        _testTemplateRepository = testTemplateRepository;
        _testTemplateConfigRepository = testTemplateConfigRepository;
        _subContentRepository = subContentRepository;
        _testQuestionRepository = testQuestionRepository;
        _testTemplateTypeRepository = testTemplateTypeRepository;
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

            var now = DateTime.UtcNow;
            if (test.availableFrom.HasValue && now < test.availableFrom.Value)
                throw ApiException.BadRequest("TEST_NOT_AVAILABLE", "Test is not yet available.");
            if (test.availableTo.HasValue && now > test.availableTo.Value)
                throw ApiException.BadRequest("TEST_EXPIRED", "Test is no longer available.");
            if (test.status == TestStatus.Close)
                throw ApiException.BadRequest("TEST_CLOSED", "Test is closed.");
          
            var userAttempts = await _testAttemptRepository.GetAllAsync(a => a.testId == dto.TestId && a.userId == dto.UserId);
            if (userAttempts.Count >= test.maxAttempts)
                throw ApiException.BadRequest("MAX_ATTEMPTS_REACHED", "User has reached the maximum number of attempts.");

            if (test.lessonId.HasValue)
            {
                var lesson = await _lessonRepository.GetByIdAsync(test.lessonId.Value);
                if (lesson == null)
                    throw ApiException.NotFound("Lesson", test.lessonId.Value);

                var enrolled = await _enrollmentRepository.GetFirstOrDefaultAsync(
                    e => e.userId == dto.UserId && e.courseId == lesson.courseId);

                if (enrolled == null)
                    throw ApiException.Forbidden("USER_NOT_ENROLLED", "User is not enrolled in the course for this test.");
            }

            var attempt = new TestAttempt
            {
                attemptId = Guid.NewGuid(),
                userId = dto.UserId,
                testId = dto.TestId,
                startTime = now,
                endTime = now.AddMinutes(test.durationMinutes + 1), // 1 minute system load
                attemptNumber = userAttempts.Count + 1,
                status = TestAttemptStatus.InProgress,
                isPass = null
            };

            await _testAttemptRepository.InsertAsync(attempt);
            await _testAttemptRepository.SaveChangesAsync();

            _autoSubmitController.AddAttempt(attempt.attemptId, attempt.endTime);
            _logger.LogInformation("Started test attempt {AttemptId} for user {UserId} and registered for auto-submit monitoring",
                attempt.attemptId, attempt.userId);

            return MapToDto(attempt);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_ATTEMPT_ERROR", $"An error occurred while starting test attempt: {ex.Message}");
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

            // Calculate and save score summary for this attempt
            await CalculateAndSaveTestScoreSummaryAsync(attempt, test);

            attempt.status = TestAttemptStatus.Completed;
            await _testAttemptRepository.UpdateAsync(attempt);
            await _testAttemptRepository.SaveChangesAsync();

            return MapToDto(attempt);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_ATTEMPT_ERROR", $"An error occurred while submitting test attempt: {ex.Message}");
        }
    }

    /// <summary>
    /// Calculates and saves the TestScoreSummary for a test attempt.
    /// </summary>
    private async Task CalculateAndSaveTestScoreSummaryAsync(TestAttempt attempt, Test test)
    {
        try
        {
            // 1. Get all test questions for the test, including Question and SubContent
            var testQuestions = await _testQuestionRepository.GetAllAsync(
                tq => tq.testId == test.testId,
                "Question.SubContent"
            );

            // 2. Calculate max scores for each content name
            int kanjiMax = testQuestions.Where(q => q.Question?.SubContent?.ContentName == ContentName.Kanji).Sum(q => q.Question?.points ?? 0);
            int vocabMax = testQuestions.Where(q => q.Question?.SubContent?.ContentName == ContentName.Vocabulary).Sum(q => q.Question?.points ?? 0);
            int grammarMax = testQuestions.Where(q => q.Question?.SubContent?.ContentName == ContentName.Grammar).Sum(q => q.Question?.points ?? 0);
            int readingMax = testQuestions.Where(q => q.Question?.SubContent?.ContentName == ContentName.Reading).Sum(q => q.Question?.points ?? 0);
            int listeningMax = testQuestions.Where(q => q.Question?.SubContent?.ContentName == ContentName.Listening).Sum(q => q.Question?.points ?? 0);
            int totalMax = kanjiMax + vocabMax + grammarMax + readingMax + listeningMax;

            // 3. Get all attempt answers for this attempt, including Question and SubContent
            var attemptAnswers = await _attemptAnswerRepository.GetAllAsync(
                a => a.attemptId == attempt.attemptId,
                "Question.SubContent"
            );

            // 4. Calculate user scores for each content name (only add score if isCorrect)
            int kanjiScore = attemptAnswers.Where(a => a.isCorrect && a.Question?.SubContent?.ContentName == ContentName.Kanji).Sum(a => a.score);
            int vocabScore = attemptAnswers.Where(a => a.isCorrect && a.Question?.SubContent?.ContentName == ContentName.Vocabulary).Sum(a => a.score);
            int grammarScore = attemptAnswers.Where(a => a.isCorrect && a.Question?.SubContent?.ContentName == ContentName.Grammar).Sum(a => a.score);
            int readingScore = attemptAnswers.Where(a => a.isCorrect && a.Question?.SubContent?.ContentName == ContentName.Reading).Sum(a => a.score);
            int listeningScore = attemptAnswers.Where(a => a.isCorrect && a.Question?.SubContent?.ContentName == ContentName.Listening).Sum(a => a.score);
            int totalScore = kanjiScore + vocabScore + grammarScore + readingScore + listeningScore;

            // 5. Create and save TestScoreSummary for this attempt
            var scoreSummary = new TestScoreSummary
            {
                TestScoreSummaryId = Guid.NewGuid(),
                TestId = test.testId,
                TestAttemptId = attempt.attemptId,
                kanji_max_score = kanjiMax,
                vocab_max_score = vocabMax,
                grammar_max_score = grammarMax,
                reading_max_score = readingMax,
                listening_max_score = listeningMax,
                kanji_score = kanjiScore,
                vocab_score = vocabScore,
                grammar_score = grammarScore,
                reading_score = readingScore,
                listening_score = listeningScore,
                total_max_score = totalMax,
                total_score = totalScore
            };

            await _testScoreSummaryRepository.InsertAsync(scoreSummary);
            await _testScoreSummaryRepository.SaveChangesAsync();

            // 6. Decide pass/fail based on test type
            bool? isPass = null;
            decimal percentScore = totalMax > 0 ? (decimal)totalScore / totalMax * 100 : 0;

            if (test.testType == TestType.CustomManual)
            {
                isPass = percentScore >= test.passing_percentage;
            }
            else if (test.testType == TestType.JLPTAuto)
            {
                var templateType = test.TestTemplateTypeId.HasValue
                    ? await _testTemplateTypeRepository.GetByIdAsync(test.TestTemplateTypeId.Value)
                    : null;

                if (templateType != null)
                {
                    isPass = percentScore >= templateType.totalPassPercentage;
                }

                var testTemplates = await _testTemplateRepository.GetAllAsync(
                    t => t.TestTemplateTypeId == test.TestTemplateTypeId,
                    "TestTemplateConfigs.SubContent"
                );

                foreach (var template in testTemplates)
                {
                    var contentNames = template.TestTemplateConfigs
                        .Select(cfg => cfg.SubContent.ContentName)
                        .Distinct()
                        .ToList();

                    foreach (var contentName in contentNames)
                    {
                        int userScore = contentName switch
                        {
                            ContentName.Kanji => kanjiScore,
                            ContentName.Vocabulary => vocabScore,
                            ContentName.Grammar => grammarScore,
                            ContentName.Reading => readingScore,
                            ContentName.Listening => listeningScore,
                            _ => 0
                        };
                        int maxScore = contentName switch
                        {
                            ContentName.Kanji => kanjiMax,
                            ContentName.Vocabulary => vocabMax,
                            ContentName.Grammar => grammarMax,
                            ContentName.Reading => readingMax,
                            ContentName.Listening => listeningMax,
                            _ => 0
                        };
                        decimal contentPercent = maxScore > 0 ? (decimal)userScore / maxScore * 100 : 0;
                        if (contentPercent < template.toPassPercentage)
                        {
                            isPass = false;
                            break;
                        }
                    }
                }
            }
            else if (test.testType == TestType.CustomAuto)
            {
                isPass = null;
            }

            attempt.isPass = isPass;
            await _testAttemptRepository.UpdateAsync(attempt);
            await _testAttemptRepository.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("CALCULATE_SCORE_SUMMARY_ERROR", $"An error occurred while calculating score summary: {ex.Message}");
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
            throw ApiException.InternalServerError("GET_ATTEMPTS_ERROR", $"An error occurred while retrieving test attempts: {ex.Message}");
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
            throw ApiException.InternalServerError("UPDATE_ATTEMPT_STATUS_ERROR", $"An error occurred while updating test attempt status: {ex.Message}");
        }
    }

    /// <summary>
    /// Get a test attempt by ID with its associated score summary.
    /// </summary>
    public async Task<(TestAttemptDto Attempt, TestScoreSummary? ScoreSummary)> GetAttemptWithScoreSummaryAsync(Guid attemptId)
    {
        try
        {
            var attempt = await _testAttemptRepository.GetByIdAsync(attemptId);
            if (attempt == null)
                throw ApiException.NotFound("TestAttempt", attemptId);

            var scoreSummary = await _testScoreSummaryRepository.GetFirstOrDefaultAsync(
                s => s.TestAttemptId == attemptId);

            return (MapToDto(attempt), scoreSummary);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("GET_ATTEMPT_SCORE_SUMMARY_ERROR", $"An error occurred while retrieving attempt and score summary: {ex.Message}");
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
            IsPass = attempt.isPass
        };
    }
}