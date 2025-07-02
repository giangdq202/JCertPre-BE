using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(JCertPreDatabaseContext context) : base(context)
        {
        }
        public async Task<List<User>> GetAcademicManagersAsync()
        {
            return await _dbSet
                .Include(u => u.Role)
                .Where(u => u.Role != null && u.Role.roleName == "Academic Manager")
                .ToListAsync();
        }
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.Role) // Tải Role cùng với User
                .FirstOrDefaultAsync(u => u.userId == userId);
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Include(u => u.Role)
                               .FirstOrDefaultAsync(u => u.userId == userId, cancellationToken);
        }
    }
}
