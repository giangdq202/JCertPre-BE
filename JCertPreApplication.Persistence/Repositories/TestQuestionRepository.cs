using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for TestQuestion entity.
    /// </summary>
    public class TestQuestionRepository : GenericRepository<TestQuestion>, ITestQuestionRepository
    {
        public TestQuestionRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task<bool> AnyAsync(Expression<Func<TestQuestion, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}