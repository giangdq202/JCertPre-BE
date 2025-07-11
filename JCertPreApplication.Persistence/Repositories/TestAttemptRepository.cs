using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TestAttemptRepository : GenericRepository<TestAttempt>, ITestAttemptRepository
    {
        public TestAttemptRepository(JCertPreDatabaseContext context) : base(context) { }
        // Implement custom methods if needed
    }
}