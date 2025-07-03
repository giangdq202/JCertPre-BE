using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface IChoiceRepository
    {
        Task<IEnumerable<Choice>> GetByQuestionIdAsync(Guid questionId);
        Task<Choice?> GetByIdAsync(Guid choiceId);
        Task<Choice> AddAsync(Guid questionId, Choice choice);
        Task UpdateListAsync(Guid questionId, IEnumerable<Choice> choices);
        Task DeleteAsync(Guid questionId, Guid choiceId);
    }
}