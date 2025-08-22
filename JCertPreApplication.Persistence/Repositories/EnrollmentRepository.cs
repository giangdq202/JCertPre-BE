using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.userId == userId && e.courseId == courseId);
        }

        public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.userId == userId)
                .OrderByDescending(e => e.enrollDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Where(e => e.courseId == courseId)
                .OrderByDescending(e => e.enrollDate)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId)
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.enrollmentId == enrollmentId);
        }
        public async Task<bool> IsUserEnrolledInCourseAsync(Guid userId, Guid courseId)
        {
            return await _dbSet
                .AnyAsync(e => e.userId == userId && e.courseId == courseId);
        }

        public async Task<bool> IsUserEnrolledInAnyCourseAsync(Guid userId)
        {
            return await _context.Enrollments.AnyAsync(e => e.userId == userId);
        }

        public async Task<long> GetTotalEnrollmentsCountAsync()
        {
            return await _context.Enrollments.LongCountAsync();
        }

        public async Task<IEnumerable<MonthlyCount>> GetEnrollmentCountsByMonthAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Enrollments
                .Where(e => e.enrollDate >= startDate && e.enrollDate < endDate)
                .GroupBy(e => new { e.enrollDate.Year, e.enrollDate.Month })
                .Select(g => new MonthlyCount(g.Key.Year, g.Key.Month, g.LongCount()))
                .ToListAsync();
        }

        public async Task<long> CountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Enrollments
                .Where(e => e.enrollDate >= startDate && e.enrollDate < endDate)
                .LongCountAsync();
        }
    }
}