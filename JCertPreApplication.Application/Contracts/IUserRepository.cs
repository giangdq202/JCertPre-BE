using JCertPreApplication.Domain.Entities;
using System.Runtime.CompilerServices;

namespace JCertPreApplication.Application.Contracts
{
    public interface IUserRepository : IGenericRepository<User>
    {
        
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<User>> GetAcademicManagersAsync();
    }
}
