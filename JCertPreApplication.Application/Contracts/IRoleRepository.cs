using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetByRoleNameAsync(string roleName);
    }
}
