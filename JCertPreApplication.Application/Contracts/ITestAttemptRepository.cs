using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ITestAttemptRepository : IGenericRepository<TestAttempt>
    {
        // Add custom methods if needed, e.g., for auto-submit or batch operations
    }
}