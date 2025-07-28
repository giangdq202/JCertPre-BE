using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class LessonProgressRepository : GenericRepository<LessonProgress>, ILessonProgressRepository
    {
        public LessonProgressRepository(JCertPreDatabaseContext context) : base(context) { }

        public async Task<List<LessonProgress>> GetByUserAndCourseAsync(Guid userId, Guid courseId)
        {
            return await _dbSet
                .Include(lp => lp.Lesson)
                .Include(lp => lp.User)
                .Where(lp => lp.userId == userId && lp.Lesson.courseId == courseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId)
        {
            return await _dbSet
                .Include(lp => lp.Lesson)
                .Include(lp => lp.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(lp => lp.userId == userId && lp.lessonId == lessonId);
        }

        public async Task<decimal> GetUserCourseCompletionRateAsync(Guid userId, Guid courseId)
        {
            // Get all lesson IDs for the course
            var lessonIds = await _context.Set<Lesson>()
                .Where(l => l.courseId == courseId)
                .Select(l => l.lessonId)
                .ToListAsync();

            if (lessonIds.Count == 0) return 0.0m;

            // Get count of user's progresses for those lessons
            var completedLessons = await _dbSet
                .Where(lp => lp.userId == userId && lessonIds.Contains(lp.lessonId))
                .CountAsync();

            return Math.Round((decimal)completedLessons / lessonIds.Count * 100, 2);
        }
    }
}