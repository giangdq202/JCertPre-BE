using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task<Pagination<Feedback>> GetPagingByCourseIdAsync(Guid courseId, int pageIndex, int pageSize, string includeProperties = "")
        {
            IQueryable<Feedback> query = _dbSet.Where(f => f.courseId == courseId)
                                               .OrderByDescending(f => f.createdAt);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            return new Pagination<Feedback>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = totalItems,
                Items = items
            };
        }

        public async Task<Feedback?> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            // Eager load User for mapping user info if needed
            return await _dbSet.Include(f => f.User)
                               .FirstOrDefaultAsync(f => f.userId == userId && f.courseId == courseId);
        }

        public async Task<decimal> GetCourseAverageRatingAsync(Guid courseId)
        {
            var ratings = await _dbSet.Where(f => f.courseId == courseId).Select(f => f.rating).ToListAsync();
            if (ratings.Count == 0) return 0;
            return Math.Round(ratings.Average(), 2);
        }
    }
}