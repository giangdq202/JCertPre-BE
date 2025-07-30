using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TestAttemptRepository : GenericRepository<TestAttempt>, ITestAttemptRepository
    {
        public TestAttemptRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task<bool> AnyAsync(Expression<Func<TestAttempt, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}