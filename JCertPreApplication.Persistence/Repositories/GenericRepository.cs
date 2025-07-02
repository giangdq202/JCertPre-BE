using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JCertPreApplication.Persistence.Repositories
{
    public  class GenericRepository<T> : IGenericRepository<T>
    where T : class
    {
        protected readonly JCertPreDatabaseContext _context;
        internal DbSet<T> _dbSet;

        public GenericRepository(JCertPreDatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var guidId = id is Guid ? (Guid)id : Guid.Empty; // Xử lý kiểu dữ liệu
            return await _dbSet.FindAsync(guidId);
        }

        public async Task<List<T>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet.Where(filter);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public Task<IQueryable<T>> GetAll()
        {
            return Task.FromResult(_dbSet.AsNoTracking());
        }

        public async Task<T> InsertAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var entry = await _dbSet.AddAsync(entity);
            return entry.Entity;
        }

        public Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }
            return await query.AsNoTracking().FirstOrDefaultAsync() ?? throw new InvalidOperationException("No entity found.");
        }

        public async Task<Pagination<T>> GetPaginationAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null, int pageIndex = 1, int pageSize = 10)
        {
            if (pageIndex < 1) throw new ArgumentException("PageIndex must be greater than 0.", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than 0.", nameof(pageSize));

            IQueryable<T> query = _dbSet;
            if (predicate != null) query = query.Where(predicate);
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new Pagination<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = totalItems,
                Items = items
            };
        }
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("The entity was modified by another user. Please refresh and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            await _dbSet.AddRangeAsync(entities);
        }
    }
}
