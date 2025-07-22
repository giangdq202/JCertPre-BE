using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TestScoreSummaryRepository : GenericRepository<TestScoreSummary>, ITestScoreSummaryRepository
    {
        public TestScoreSummaryRepository(JCertPreDatabaseContext context) : base(context) { }
        // Implement custom methods if needed
    }
}