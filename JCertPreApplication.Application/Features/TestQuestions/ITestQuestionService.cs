using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Utilities;

public interface ITestQuestionService
{
    Task AddQuestionToTestAsync(Guid testId, Guid questionId);
    Task<Pagination<TestQuestion>> GetQuestionsByTestIdAsync(Guid testId, int pageIndex, int pageSize);
    Task<TestQuestion?> GetTestQuestionAsync(Guid testId, Guid questionId);
    Task<List<Guid>> GetAllQuestionIdsByTestIdAsync(Guid testId);
    Task UpdateIsActiveAsync(Guid testId, Guid questionId, bool isActive);
    Task DeleteTestQuestionAsync(Guid testId, Guid questionId);
}