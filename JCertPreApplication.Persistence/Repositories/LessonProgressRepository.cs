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
            // Get total lessons for the course (count only, no list)
            var totalLessons = await _context.Set<Lesson>()
                .Where(l => l.courseId == courseId)
                .CountAsync();

            if (totalLessons == 0) return 0.0m;

            // Get count of user's progresses for those lessons (join for efficiency)
            var completedLessons = await (from lp in _dbSet
                                          join l in _context.Set<Lesson>() on lp.lessonId equals l.lessonId
                                          where lp.userId == userId && l.courseId == courseId
                                          select lp.lessonId)
                                         .Distinct()
                                         .CountAsync();

            return Math.Round((decimal)completedLessons / totalLessons * 100, 2);
        }
    }
}