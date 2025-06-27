using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetByRoleNameAsync(string roleName);
    }
}
