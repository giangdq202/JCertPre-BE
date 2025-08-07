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
    private readonly IQuestionRepository _questionRepository;
    private readonly ITestTemplateTypeRepository _testTemplateTypeRepository;
    private readonly ITestTemplateRepository _testTemplateRepository;
    private readonly ITestTemplateConfigRepository _testTemplateConfigRepository;

    public TestQuestionService(
        ITestQuestionRepository testQuestionRepo,
        ITestRepository testRepo,
        ITestAttemptRepository testAttemptRepo,
        IQuestionRepository questionRepository,
        ITestTemplateTypeRepository testTemplateTypeRepo,
        ITestTemplateRepository testTemplateRepo,
        ITestTemplateConfigRepository testTemplateConfigRepo)
    {
        _testQuestionRepo = testQuestionRepo;
        _testRepo = testRepo;
        _testAttemptRepo = testAttemptRepo;
        _questionRepository = questionRepository;
        _testTemplateTypeRepository = testTemplateTypeRepo;
        _testTemplateRepository = testTemplateRepo;
        _testTemplateConfigRepository = testTemplateConfigRepo;
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

                // Get all existing questionIds for this test
                var existingQuestions = await _testQuestionRepo.GetAllAsync(tq => tq.testId == testId);
                var existingQuestionIds = existingQuestions.Select(q => q.questionId).ToHashSet();

                int startNumber = existingQuestions.Count + 1;

                var entities = new List<TestQuestion>();
                foreach (var pair in group)
                {
                    // Skip if questionId already exists in this test
                    if (existingQuestionIds.Contains(pair.questionId))
                        continue;

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

                    // Add to set to prevent duplicates in the same batch
                    existingQuestionIds.Add(pair.questionId);
                }

                if (entities.Count == 0)
                    continue; // No active or unique questions to add for this test

                await _testQuestionRepo.AddRangeAsync(entities);
                await _testQuestionRepo.SaveChangesAsync();
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
            return testQuestions
                .OrderBy(q => q.questionNumber)
                .Select(MapToTestQuestionDto)
                .ToList();
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

            await ReorderTestQuestionNumbersAsync(testQuestion.testId);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while deleting the test question: {ex.Message}");
        }
    }

    public async Task DeleteAllTestQuestionsAsync(Guid testId)
    {
        try
        {
            var test = await _testRepo.GetByIdAsync(testId);
            if (test == null)
                throw ApiException.NotFound("Test", testId);

            if (test.status != TestStatus.Close)
                throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Can only delete questions if the test status is closed.");

            var now = DateTime.UtcNow;
            var activeAttemptExists = await _testAttemptRepo.AnyAsync(
                ta => ta.testId == testId && ta.startTime <= now && ta.endTime >= now);

            if (activeAttemptExists)
                throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Cannot delete questions during an active test attempt window.");

            var testQuestions = await _testQuestionRepo.GetAllAsync(tq => tq.testId == testId);
            if (!testQuestions.Any())
                return;

            foreach (var tq in testQuestions)
            {
                await _testQuestionRepo.DeleteAsync(tq);
            }
            await _testQuestionRepo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while deleting all test questions: {ex.Message}");
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

    public async Task AddQuestionsJLPTAutoAsync(Guid testId)
    {
        var test = await _testRepo.GetByIdAsync(testId)
            ?? throw ApiException.NotFound("Test", testId);

        if (!test.TestTemplateTypeId.HasValue)
            throw ApiException.BadRequest("NO_TEMPLATE_TYPE", "TestTemplateTypeId is required.");

        var templateType = await _testTemplateTypeRepository.GetByIdAsync(test.TestTemplateTypeId.Value)
            ?? throw ApiException.NotFound("TestTemplateType", test.TestTemplateTypeId.Value);

        var testTemplates = await _testTemplateRepository.GetAllAsync(
            t => t.TestTemplateTypeId == test.TestTemplateTypeId.Value);
        var orderedTemplates = testTemplates.OrderBy(t => t.sequence).ToList();

        int questionNumber = 1;
        var testQuestionsToAdd = new List<TestQuestion>();

        foreach (var template in orderedTemplates)
        {
            int partNumber = template.sequence;
            int partDuration = template.durationMinutes;

            var configs = await _testTemplateConfigRepository.GetAllAsync(
                c => c.templateId == template.templateId);
            var orderedConfigs = configs.OrderBy(c => c.sequence).ToList();

            foreach (var config in orderedConfigs)
            {
                // Use the new repo method to get unique random question IDs
                var randomIds = await _questionRepository.GetRandomQuestionIdsAsync(
                    config.subContentId, config.questionCount, config.pointPerQuestion);

                foreach (var qid in randomIds)
                {
                    testQuestionsToAdd.Add(new TestQuestion
                    {
                        testQuestionId = Guid.NewGuid(),
                        testId = testId,
                        questionId = qid,
                        questionNumber = questionNumber++,
                        partNumber = partNumber,
                        partDurationMinutes = partDuration
                    });
                }
            }
        }

        if (testQuestionsToAdd.Count == 0)
            throw ApiException.BadRequest("NO_QUESTIONS_FOUND", "No questions found for JLPTAuto generation.");

        await _testQuestionRepo.AddRangeAsync(testQuestionsToAdd);
        await _testQuestionRepo.SaveChangesAsync();
    }
}