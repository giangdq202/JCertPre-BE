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

        public async Task<(int LessonOrder, decimal CompletionRate)?> GetHighestPreviousLessonProgressAsync(Guid userId, Guid courseId)
        {
            // Get the highest lesson order and its completion rate for the user's progress in the course
            var result = await (from lp in _dbSet
                                join l in _context.Set<Lesson>() on lp.lessonId equals l.lessonId
                                where lp.userId == userId && l.courseId == courseId
                                orderby l.lessonOrder descending
                                select new { l.lessonOrder, lp.completionRate })
                               .FirstOrDefaultAsync();

            return result == null ? null : (result.lessonOrder, result.completionRate);
        }

        public async Task<decimal> CalculateCompletionRateAfterAddAsync(Guid userId, Guid courseId)
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

            // Add 1 for the new progress
            return Math.Round((decimal)(completedLessons + 1) / totalLessons * 100, 2);
        }

    }
}