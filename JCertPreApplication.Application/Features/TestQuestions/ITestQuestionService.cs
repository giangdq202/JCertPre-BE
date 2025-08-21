using JCertPreApplication.Application.Dtos.TestQuestion;

namespace JCertPreApplication.Application.Features.TestQuestions;
public interface ITestQuestionService
{
    Task AddQuestionsCustomManualAsync(List<(Guid testId, Guid questionId)> testQuestionPairs);
    Task<List<TestQuestionDto>> GetQuestionsByTestIdAsync(Guid testId);
    Task DeleteTestQuestionAsync(Guid testQuestionId);
    Task AddQuestionsJLPTAutoAsync(Guid testId);
    Task DeleteAllTestQuestionsAsync(Guid testId);
}