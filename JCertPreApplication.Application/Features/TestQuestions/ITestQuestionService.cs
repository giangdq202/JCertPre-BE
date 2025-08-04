using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITestQuestionService 
{
    Task AddQuestionsCustomManualAsync(List<(Guid testId, Guid questionId)> testQuestionPairs);
    Task<List<TestQuestionDto>> GetQuestionsByTestIdAsync(Guid testId);
    Task DeleteTestQuestionAsync(Guid testQuestionId);
    Task CalculateAndUpdateTestScoreSummaryMaxScoresAsync(Guid testId);
    Task AddQuestionsJLPTAutoAsync(Guid testId);
}