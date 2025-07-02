using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<Role?> GetByRoleNameAsync(string roleName)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.roleName == roleName);
        }
    }
}
