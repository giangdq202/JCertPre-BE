using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JCertPreApplication.Persistence.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task DeleteAllByCourseIdAsync(Guid courseId)
        {
            var lessons = await _dbSet.Where(l => l.courseId == courseId).ToListAsync();
            _dbSet.RemoveRange(lessons);
        }
        public async Task<Pagination<Lesson>> GetPaginatedLessonsByCourseAsync(
        Guid courseId,
        string? searchTerm,
        int pageIndex,
        int pageSize)
        {
            if (pageIndex < 1) throw new ArgumentException("PageIndex must be greater than 0.", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than 0.", nameof(pageSize));

            IQueryable<Lesson> query = _dbSet.Where(l =>
                l.courseId == courseId &&
                (string.IsNullOrEmpty(searchTerm) || l.title.ToLower().Contains(searchTerm.ToLower())));

            // Order by LessonOrder ascending (min to max)
            query = query.OrderBy(l => l.lessonOrder);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new Pagination<Lesson>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = totalItems,
                Items = items
            };
        }

        public async Task<Test?> GetTestByLessonIdAsync(Guid lessonId)
        {
            return await _context.Set<Test>().FirstOrDefaultAsync(t => t.lessonId == lessonId);
        }

        public async Task<bool> IsUserPassedTestAsync(Guid userId, Guid testId)
        {
            return await _context.Set<TestAttempt>()
                .AnyAsync(a => a.testId == testId && a.userId == userId && a.isPass == true);
        }

        public async Task<int> CountAsync(Expression<Func<Lesson, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
    }
}