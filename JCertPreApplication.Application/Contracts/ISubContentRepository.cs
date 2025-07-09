using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    /// <summary>
    /// Repository interface for SubContent entity, extends generic repository.
    /// </summary>
    public interface ISubContentRepository : IGenericRepository<SubContent>
    {
        // Add custom methods for SubContent if needed in the future.
    }
}