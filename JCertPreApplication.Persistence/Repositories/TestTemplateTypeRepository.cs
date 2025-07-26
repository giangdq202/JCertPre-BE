using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TestTemplateTypeRepository : GenericRepository<TestTemplateType>, ITestTemplateTypeRepository
    {
        public TestTemplateTypeRepository(JCertPreDatabaseContext context) : base(context) { }
        // Add custom methods if needed
    }
}