using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class LivestreamRepository : GenericRepository<Livestream>, ILivestreamRepository
    {
        public LivestreamRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<Livestream?> GetLivestreamWithDetailsAsync(Guid livestreamId)
        {
            return await _dbSet
                .Include(ls => ls.Course)
                    .ThenInclude(c => c.CourseInstructors.Where(ci => ci.IsActive))
                    .ThenInclude(ci => ci.Instructor)
                .Include(ls => ls.Course)
                    .ThenInclude(c => c.Enrollments)
                .FirstOrDefaultAsync(ls => ls.livestreamId == livestreamId);
        }

        public async Task<List<Livestream>> GetLivestreamsByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Include(ls => ls.Course)
                .Where(ls => ls.courseId == courseId)
                .OrderBy(ls => ls.scheduledDateTime)
                .ToListAsync();
        }

        public async Task<List<Livestream>> GetLivestreamsByUserAsync(Guid userId)
        {
            // Get livestreams from courses where user is either instructor or student
            var instructorCourseIds = await _context.Set<CourseInstructor>()
                .Where(ci => ci.InstructorId == userId && ci.IsActive)
                .Select(ci => ci.CourseId)
                .ToListAsync();

            var studentCourseIds = await _context.Set<Enrollment>()
                .Where(e => e.userId == userId)
                .Select(e => e.courseId)
                .ToListAsync();

            var allCourseIds = instructorCourseIds.Union(studentCourseIds).ToList();

            return await _dbSet
                .Include(ls => ls.Course)
                .Where(ls => allCourseIds.Contains(ls.courseId))
                .OrderBy(ls => ls.scheduledDateTime)
                .ToListAsync();
        }

        public async Task<Pagination<Livestream>> GetLivestreamsWithPaginationAsync(
            Guid? courseId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            if (pageIndex < 1) throw new ArgumentException("PageIndex must be greater than 0.", nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than 0.", nameof(pageSize));

            IQueryable<Livestream> query = _dbSet
                .Include(ls => ls.Course);

            // Filter by course
            if (courseId.HasValue)
            {
                query = query.Where(ls => ls.courseId == courseId.Value);
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                query = query.Where(ls => ls.scheduledDateTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(ls => ls.scheduledDateTime <= endDate.Value);
            }

            // Order by scheduled date
            query = query.OrderBy(ls => ls.scheduledDateTime);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new Pagination<Livestream>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItemsCount = totalItems,
                Items = items
            };
        }

        public async Task<bool> HasScheduleConflictAsync(Guid courseId, DateTime scheduledDateTime, int durationMinutes, Guid? excludeLivestreamId = null)
        {
            var endDateTime = scheduledDateTime.AddMinutes(durationMinutes);

            var query = _dbSet.Where(ls => 
                ls.courseId == courseId &&
                ls.status != Domain.Enums.LivestreamStatus.COMPLETED);

            if (excludeLivestreamId.HasValue)
            {
                query = query.Where(ls => ls.livestreamId != excludeLivestreamId.Value);
            }

            return await query.AnyAsync(ls =>
                (scheduledDateTime >= ls.scheduledDateTime && scheduledDateTime < ls.scheduledDateTime.AddMinutes(ls.durationMinutes)) ||
                (endDateTime > ls.scheduledDateTime && endDateTime <= ls.scheduledDateTime.AddMinutes(ls.durationMinutes)) ||
                (scheduledDateTime <= ls.scheduledDateTime && endDateTime >= ls.scheduledDateTime.AddMinutes(ls.durationMinutes))
            );
        }
    }
}
