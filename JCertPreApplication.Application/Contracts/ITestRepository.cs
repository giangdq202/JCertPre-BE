using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ITestRepository : IGenericRepository<Test>
    {
        // No custom paging method needed, use generic GetPaginationAsync
    }
}