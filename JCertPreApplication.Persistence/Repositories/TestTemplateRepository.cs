using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TestTemplateRepository : GenericRepository<TestTemplate>, ITestTemplateRepository
    {
        public TestTemplateRepository(JCertPreDatabaseContext context) : base(context) { }
        // Add custom methods if needed
    }
}