using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class TestTemplateConfigRepository : GenericRepository<TestTemplateConfig>, ITestTemplateConfigRepository
    {
        public TestTemplateConfigRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task<List<TestTemplateConfig>> GetAllByTemplateIdAsync(Guid templateId)
        {
            return await _dbSet.Where(x => x.templateId == templateId)
                .Include(x => x.SubContent)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TestTemplateConfig?> GetByConfigIdAsync(Guid configId)
        {
            return await _dbSet
                .Include(x => x.SubContent)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.configId == configId);
        }
    }
}