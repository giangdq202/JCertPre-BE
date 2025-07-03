using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Questions
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question> GetByIdAsync(Guid id);
        Task<Question> CreateAsync(Question question);
        Task UpdateAsync(Question question);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Question>> GetQuestionsWithDetailsAsync();
        Task<Pagination<Question>> GetPagingAsync(
            Expression<Func<Question, bool>>? predicate = null,
            string? includeProperties = null,
            int pageIndex = 1,
            int pageSize = 10);
    }
}