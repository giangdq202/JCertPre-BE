using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

public class TestQuestionService : ITestQuestionService
{
    private readonly ITestQuestionRepository _testQuestionRepo;
    private readonly ITestRepository _testRepo;
    private readonly IQuestionRepository _questionRepo;

    public TestQuestionService(
        ITestQuestionRepository testQuestionRepo,
        ITestRepository testRepo,
        IQuestionRepository questionRepo)
    {
        _testQuestionRepo = testQuestionRepo;
        _testRepo = testRepo;
        _questionRepo = questionRepo;
    }

    public async Task AddQuestionToTestAsync(Guid testId, Guid questionId)
    {
        try
        {
            var test = await _testRepo.GetByIdAsync(testId);
            if (test == null)
                throw ApiException.NotFound("Test", testId);

            var question = await _questionRepo.GetByIdAsync(questionId);
            if (question == null)
                throw ApiException.NotFound("Question", questionId);

            var exists = await _testQuestionRepo.GetFirstOrDefaultAsync(tq => tq.testId == testId && tq.questionId == questionId);
            if (exists != null)
                throw ApiException.BadRequest("TEST_QUESTION_EXISTS", "This question is already added to the test.");

            var testQuestion = new TestQuestion
            {
                testQuestionId = Guid.NewGuid(),
                testId = testId,
                questionId = questionId,
                isActive = true
            };
            await _testQuestionRepo.InsertAsync(testQuestion);
            await _testQuestionRepo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while adding question to test: {ex.Message}");
        }
    }

    public async Task<Pagination<TestQuestion>> GetQuestionsByTestIdAsync(Guid testId, int pageIndex, int pageSize)
    {
        try
        {
            return await _testQuestionRepo.GetPaginationAsync(
            predicate: tq => tq.testId == testId,
            includeProperties: "Question,Question.Choices",
            pageIndex: pageIndex,
            pageSize: pageSize);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while retrieving questions: {ex.Message}");
        }
    }

    public async Task<TestQuestion?> GetTestQuestionAsync(Guid testId, Guid questionId)
    {
        try
        {
            return await _testQuestionRepo.GetFirstOrDefaultAsync(
            predicate: tq => tq.testId == testId && tq.questionId == questionId,
            includeProperties: "Question,Question.Choices");
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while retrieving the test question: {ex.Message}");
        }
    }

    public async Task<List<Guid>> GetAllQuestionIdsByTestIdAsync(Guid testId)
    {
        try
        {
            var testQuestions = await _testQuestionRepo.GetAllAsync(tq => tq.testId == testId);
            return testQuestions.Select(tq => tq.questionId).ToList();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while retrieving question ids: {ex.Message}");
        }
    }

    public async Task UpdateIsActiveAsync(Guid testId, Guid questionId, bool isActive)
    {
        try
        {
            var testQuestion = await _testQuestionRepo.GetFirstOrDefaultAsync(tq => tq.testId == testId && tq.questionId == questionId);
            if (testQuestion == null)
                throw ApiException.NotFound("TestQuestion", $"TestId: {testId}, QuestionId: {questionId}");

            testQuestion.isActive = isActive;
            await _testQuestionRepo.UpdateAsync(testQuestion);
            await _testQuestionRepo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while updating isActive: {ex.Message}");
        }
    }

    public async Task DeleteTestQuestionAsync(Guid testId, Guid questionId)
    {
        try
        {
            var testQuestion = await _testQuestionRepo.GetFirstOrDefaultAsync(tq => tq.testId == testId && tq.questionId == questionId);
            if (testQuestion == null)
                throw ApiException.NotFound("TestQuestion", $"TestId: {testId}, QuestionId: {questionId}");

            await _testQuestionRepo.DeleteAsync(testQuestion);
            await _testQuestionRepo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("TEST_QUESTION_SERVICE_ERROR", $"An error occurred while deleting the test question: {ex.Message}");
        }
    }
}