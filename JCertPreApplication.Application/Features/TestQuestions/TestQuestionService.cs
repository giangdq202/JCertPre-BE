using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TestQuestionService : ITestQuestionService
{
    private readonly ITestQuestionRepository _testQuestionRepo;
    private readonly ITestRepository _testRepo;
    private readonly ITestAttemptRepository _testAttemptRepo;
    private readonly ITestScoreSummaryRepository _testScoreSummaryRepository;
    private readonly IQuestionRepository _questionRepository;

    public TestQuestionService(
        ITestQuestionRepository testQuestionRepo,
        ITestRepository testRepo,
        ITestAttemptRepository testAttemptRepo,
        ITestScoreSummaryRepository testScoreSummaryRepository,
        IQuestionRepository questionRepository)
    {
        _testQuestionRepo = testQuestionRepo;
        _testRepo = testRepo;
        _testAttemptRepo = testAttemptRepo;
        _testScoreSummaryRepository = testScoreSummaryRepository;
        _questionRepository = questionRepository;
    }

    public async Task AddQuestionsCustomManualAsync(List<(Guid testId, Guid questionId)> testQuestionPairs)
    {
        try
        {
            if (testQuestionPairs == null || !testQuestionPairs.Any())
                throw ApiException.BadRequest("NO_QUESTIONS", "No questions provided.");

            var grouped = testQuestionPairs.GroupBy(x => x.testId);

            foreach (var group in grouped)
            {
                var testId = group.Key;
                var test = await _testRepo.GetByIdAsync(testId);
                if (test == null)
                    throw ApiException.NotFound("Test", testId);

                var existingQuestions = await _testQuestionRepo.GetAllAsync(tq => tq.testId == testId);
                int startNumber = existingQuestions.Count + 1;

                var entities = new List<TestQuestion>();
                foreach (var pair in group)
                {
                    // Use question repo for faster isActive check
                    var question = await _questionRepository.GetByIdAsync(pair.questionId);
                    if (question == null || !question.isActive)
                        continue; // Skip inactive or not found questions

                    entities.Add(new TestQuestion
                    {
                        testQuestionId = Guid.NewGuid(),
                        testId = testId,
                        questionId = pair.questionId,
                        questionNumber = startNumber++
                    });
                }

                if (entities.Count == 0)
                    continue; // No active questions to add for this test

                await _testQuestionRepo.AddRangeAsync(entities);
                await _testQuestionRepo.SaveChangesAsync();

                // If only one question is added, update only the relevant field
                if (entities.Count == 1)
                {
                    var (point, contentName) = await GetQuestionPointAndContentNameByQuestionIdAsync(entities[0].questionId)
                        ?? throw ApiException.NotFound("Question", entities[0].questionId);
                    await UpdateTestScoreSummaryFieldAsync(testId, point, contentName, true);
                }
                else
                {
                    // Recalculate max scores for this test
                    await CalculateAndUpdateTestScoreSummaryMaxScoresAsync(testId);
                }
            }
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while adding questions: {ex.Message}");
        }
    }

    public async Task<List<TestQuestionDto>> GetQuestionsByTestIdAsync(Guid testId)
    {
        try
        {
            var testQuestions = await _testQuestionRepo.GetAllAsync(tq => tq.testId == testId);
            return testQuestions.Select(MapToTestQuestionDto).ToList();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while retrieving test questions: {ex.Message}");
        }
    }

    public async Task DeleteTestQuestionAsync(Guid testQuestionId)
    {
        try
        {
            var testQuestion = await _testQuestionRepo.GetByIdAsync(testQuestionId);
            if (testQuestion == null)
                throw ApiException.NotFound("TestQuestion", testQuestionId);

            var test = await _testRepo.GetByIdAsync(testQuestion.testId);
            if (test == null)
                throw ApiException.NotFound("Test", testQuestion.testId);

            if (test.status != TestStatus.Close)
                throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Can only delete question if the test status is closed.");

            var now = DateTime.UtcNow;
            var activeAttemptExists = await _testAttemptRepo.AnyAsync(
                ta => ta.testId == testQuestion.testId && ta.startTime <= now && ta.endTime >= now);

            if (activeAttemptExists)
                throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Cannot delete question during an active test attempt window.");

            int deletedQuestionNumber = testQuestion.questionNumber;

            await _testQuestionRepo.DeleteAsync(testQuestion);
            await _testQuestionRepo.SaveChangesAsync();

            var (point, contentName) = await GetQuestionPointAndContentNameByQuestionIdAsync(testQuestion.questionId)
                ?? throw ApiException.NotFound("Question", testQuestion.questionId);
            await UpdateTestScoreSummaryFieldAsync(testQuestion.testId, point, contentName, false);

            // Reorder question numbers after deletion
            await ReorderTestQuestionNumbersAsync(testQuestion.testId);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while deleting the test question: {ex.Message}");
        }
    }

    public async Task ReorderTestQuestionNumbersAsync(Guid testId)
    {
        try
        {
            // Get all questions for the test, ordered by questionNumber
            var questions = await _testQuestionRepo.GetAllAsync(tq => tq.testId == testId);
            var orderedQuestions = questions.OrderBy(q => q.questionNumber).ToList();
            if (!orderedQuestions.Any())
                return; // No questions to reorder
            int number = 1;
            foreach (var tq in orderedQuestions)
            {
                tq.questionNumber = number++;
                await _testQuestionRepo.UpdateAsync(tq);
            }
            await _testQuestionRepo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while reordering question numbers: {ex.Message}");
        }
    }

    private static TestQuestionDto MapToTestQuestionDto(TestQuestion tq)
    {
        return new TestQuestionDto
        {
            TestQuestionId = tq.testQuestionId,
            TestId = tq.testId,
            QuestionId = tq.questionId,
            QuestionNumber = tq.questionNumber,
            PartNumber = tq.partNumber,
            PartDurationMinutes = tq.partDurationMinutes
        };
    }

    public async Task CalculateAndUpdateTestScoreSummaryMaxScoresAsync(Guid testId)
    {
        // Get all test questions for the test, including Question and SubContent
        var testQuestions = await _testQuestionRepo.GetAllAsync(
            tq => tq.testId == testId,
            "Question,Question.SubContent"
        );

        // Group and sum points by ContentName
        int kanjiMax = testQuestions
            .Where(q => q.Question?.SubContent?.ContentName == ContentName.Kanji)
            .Sum(q => q.Question?.points ?? 0);

        int vocabMax = testQuestions
            .Where(q => q.Question?.SubContent?.ContentName == ContentName.Vocabulary)
            .Sum(q => q.Question?.points ?? 0);

        int grammarMax = testQuestions
            .Where(q => q.Question?.SubContent?.ContentName == ContentName.Grammar)
            .Sum(q => q.Question?.points ?? 0);

        int readingMax = testQuestions
            .Where(q => q.Question?.SubContent?.ContentName == ContentName.Reading)
            .Sum(q => q.Question?.points ?? 0);

        int listeningMax = testQuestions
            .Where(q => q.Question?.SubContent?.ContentName == ContentName.Listening)
            .Sum(q => q.Question?.points ?? 0);

        int totalMax = kanjiMax + vocabMax + grammarMax + readingMax + listeningMax;

        // Get the test score summary for this test (where TestAttemptId == null)
        var summary = await _testScoreSummaryRepository.GetFirstOrDefaultAsync(
            s => s.TestId == testId && s.TestAttemptId == null);

        if (summary == null)
            throw ApiException.NotFound("TestScoreSummary", testId);

        // Update max score fields
        summary.kanji_max_score = kanjiMax;
        summary.vocab_max_score = vocabMax;
        summary.grammar_max_score = grammarMax;
        summary.reading_max_score = readingMax;
        summary.listening_max_score = listeningMax;
        summary.total_max_score = totalMax;

        await _testScoreSummaryRepository.UpdateAsync(summary);
        await _testScoreSummaryRepository.SaveChangesAsync();
    }

    // 1. Get question point and content name by question id
    public async Task<(int Point, ContentName ContentName)?> GetQuestionPointAndContentNameByQuestionIdAsync(Guid questionId)
    {
        var question = await _testQuestionRepo.GetFirstOrDefaultAsync(
            tq => tq.questionId == questionId,
            "Question,Question.SubContent"
        );
        if (question?.Question == null || question.Question.SubContent == null)
            return null;

        return (question.Question.points, question.Question.SubContent.ContentName);
    }

    public async Task UpdateTestScoreSummaryFieldAsync(Guid testId, int questionPoint, ContentName contentName, bool isAdd)
    {
        var summary = await _testScoreSummaryRepository.GetFirstOrDefaultAsync(
            s => s.TestId == testId && s.TestAttemptId == null);

        if (summary == null)
            throw ApiException.NotFound("TestScoreSummary", testId);

        int delta = isAdd ? questionPoint : -questionPoint;

        switch (contentName)
        {
            case ContentName.Kanji:
                summary.kanji_max_score = Math.Max(0, summary.kanji_max_score + delta);
                break;
            case ContentName.Vocabulary:
                summary.vocab_max_score = Math.Max(0, summary.vocab_max_score + delta);
                break;
            case ContentName.Grammar:
                summary.grammar_max_score = Math.Max(0, summary.grammar_max_score + delta);
                break;
            case ContentName.Reading:
                summary.reading_max_score = Math.Max(0, summary.reading_max_score + delta);
                break;
            case ContentName.Listening:
                summary.listening_max_score = Math.Max(0, summary.listening_max_score + delta);
                break;
            default:
                throw ApiException.BadRequest("INVALID_CONTENT_NAME", $"ContentName {contentName} is not supported.");
        }

        summary.total_max_score = Math.Max(0,
            summary.kanji_max_score +
            summary.vocab_max_score +
            summary.grammar_max_score +
            summary.reading_max_score +
            summary.listening_max_score);

        await _testScoreSummaryRepository.UpdateAsync(summary);
        await _testScoreSummaryRepository.SaveChangesAsync();
    }
}