using JCertPreApplication.Domain.Entities;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface ITestAttemptRepository : IGenericRepository<TestAttempt>
    {
        Task<bool> AnyAsync(Expression<Func<TestAttempt, bool>> predicate);
    }
}