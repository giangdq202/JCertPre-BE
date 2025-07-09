using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;

namespace JCertPreApplication.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for SubContent entity.
    /// </summary>
    public class SubContentRepository : GenericRepository<SubContent>, ISubContentRepository
    {
        public SubContentRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        // Implement custom methods for SubContent if needed in the future.
    }
}