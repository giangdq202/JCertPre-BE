using JCertPreApplication.Application.Utilities;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Contracts
{
    public interface IGenericRepository<T>
    where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<List<T>> GetAllAsync(string? includeProperties = null);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
        Task<IQueryable<T>> GetAll();
        Task<T> InsertAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            string? includeProperties = null
        );
        Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task<int> SaveChangesAsync();
        Task<Pagination<T>> GetPaginationAsync(
            Expression<Func<T, bool>>? predicate = null,
            string? includeProperties = null,
            int pageIndex = 1,
            int pageSize = 10
        );
    }
}
