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
        ITestTemplateConfigRepository testTemplateConfigRepository)
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

            // If lessonId is set, check enrollment in the course of the lesson
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

            // Create new attempt
            var attempt = new TestAttempt
            {
                attemptId = Guid.NewGuid(),
                userId = dto.UserId,
                testId = dto.TestId,
                startTime = now,
                endTime = now.AddMinutes(test.durationMinutes + 1),//1 minunte system load
                attemptNumber = userAttempts.Count + 1,
                status = TestAttemptStatus.InProgress,
                isPass = null
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

            // Calculate and save score summary for this attempt
            await CalculateAndSaveTestScoreSummaryAsync(attempt, test);

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
    /// Calculates and saves the TestScoreSummary for a test attempt.
    /// </summary>
    private async Task CalculateAndSaveTestScoreSummaryAsync(TestAttempt attempt, Test test)
    {
        // Get all answers for this attempt, including Question and SubContent
        var answers = await _attemptAnswerRepository.GetAllAsync(
            a => a.attemptId == attempt.attemptId,
            "Question,Question.SubContent"
        );

        // Get max scores from TestScoreSummary (where TestAttemptId == null)
        var maxSummary = await _testScoreSummaryRepository.GetFirstOrDefaultAsync(
            s => s.TestId == test.testId && s.TestAttemptId == null);

        if (maxSummary == null)
            throw ApiException.NotFound("TestScoreSummary", test.testId);

        // Prepare new summary for this attempt
        var grouped = answers
            .Where(a => a.isCorrect && a.Question != null && a.Question.SubContent != null)
            .GroupBy(a => a.Question.SubContent.ContentName)
            .ToDictionary(g => g.Key, g => g.Sum(a => a.score));

        var summary = new TestScoreSummary
        {
            TestScoreSummaryId = Guid.NewGuid(),
            TestId = test.testId,
            TestAttemptId = attempt.attemptId,
            kanji_score = grouped.TryGetValue(ContentName.Kanji, out var kanji) ? kanji : 0,
            vocab_score = grouped.TryGetValue(ContentName.Vocabulary, out var vocab) ? vocab : 0,
            grammar_score = grouped.TryGetValue(ContentName.Grammar, out var grammar) ? grammar : 0,
            reading_score = grouped.TryGetValue(ContentName.Reading, out var reading) ? reading : 0,
            listening_score = grouped.TryGetValue(ContentName.Listening, out var listening) ? listening : 0,
            kanji_max_score = maxSummary.kanji_max_score,
            vocab_max_score = maxSummary.vocab_max_score,
            grammar_max_score = maxSummary.grammar_max_score,
            reading_max_score = maxSummary.reading_max_score,
            listening_max_score = maxSummary.listening_max_score,
            total_score = grouped.Values.Sum(),
            total_max_score = maxSummary.total_max_score,
            passing_percentage = maxSummary.passing_percentage
        };

        summary.percentage_score = summary.total_max_score > 0
            ? Math.Round((decimal)summary.total_score * 100 / summary.total_max_score, 2)
            : 0m;

        bool isPass = false;

        if (test.testType == TestType.CustomManual)
        {
            isPass = summary.percentage_score >= summary.passing_percentage;
        }
        else if (test.testType == TestType.JLPTAuto && test.TestTemplateTypeId.HasValue)
        {
            // For JLPTAuto-like logic: check each test template's toPassPercentage
            var testTemplates = await _testTemplateRepository.GetAllAsync(
                t => t.TestTemplateTypeId == test.TestTemplateTypeId.Value);

            isPass = true; // Assume pass unless any template fails

            foreach (var template in testTemplates)
            {
                var configs = await _testTemplateConfigRepository.GetAllAsync(
                    c => c.templateId == template.templateId);

                // Get all subContentIds for this template
                var subContentIds = configs.Select(c => c.subContentId).ToHashSet();

                // Map subContentId to ContentName
                // Fetch all questions for these subContentIds in one query
                var questions = await _questionRepository.GetAllAsync(
                    q => subContentIds.Contains(q.SubContentId), "SubContent");
                var subContentNames = questions
                    .Where(q => q.SubContent != null)
                    .Select(q => q.SubContent.ContentName)
                    .ToHashSet();

                // Sum max scores for this template
                int templateMaxScore = 0;
                int templateUserScore = 0;

                foreach (var contentName in subContentNames)
                {
                    switch (contentName)
                    {
                        case ContentName.Kanji:
                            templateMaxScore += maxSummary.kanji_max_score;
                            templateUserScore += summary.kanji_score;
                            break;
                        case ContentName.Vocabulary:
                            templateMaxScore += maxSummary.vocab_max_score;
                            templateUserScore += summary.vocab_score;
                            break;
                        case ContentName.Grammar:
                            templateMaxScore += maxSummary.grammar_max_score;
                            templateUserScore += summary.grammar_score;
                            break;
                        case ContentName.Reading:
                            templateMaxScore += maxSummary.reading_max_score;
                            templateUserScore += summary.reading_score;
                            break;
                        case ContentName.Listening:
                            templateMaxScore += maxSummary.listening_max_score;
                            templateUserScore += summary.listening_score;
                            break;
                    }
                }

                decimal templatePercentage = templateMaxScore > 0
                    ? Math.Round((decimal)templateUserScore * 100 / templateMaxScore, 2)
                    : 0m;

                if (templatePercentage < template.toPassPercentage)
                {
                    isPass = false;
                    break;
                }
            }
        }

        attempt.isPass = isPass;

        await _testScoreSummaryRepository.InsertAsync(summary);
        await _testScoreSummaryRepository.SaveChangesAsync();

        await _testAttemptRepository.UpdateAsync(attempt);
        await _testAttemptRepository.SaveChangesAsync();
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

    /// <summary>
    /// Get a test attempt by ID with its associated score summary.
    /// </summary>
    public async Task<(TestAttemptDto Attempt, TestScoreSummary? ScoreSummary)> GetAttemptWithScoreSummaryAsync(Guid attemptId)
    {
        var attempt = await _testAttemptRepository.GetByIdAsync(attemptId);
        if (attempt == null)
            throw ApiException.NotFound("TestAttempt", attemptId);

        var scoreSummary = await _testScoreSummaryRepository.GetFirstOrDefaultAsync(
            s => s.TestAttemptId == attemptId);

        return (MapToDto(attempt), scoreSummary);
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