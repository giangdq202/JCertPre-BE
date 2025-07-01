using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<Course?> GetCourseWithDetailsAsync(Guid courseId)
        {
            return await _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Lessons)
                .Include(c => c.Livestreams)
                .Include(c => c.Enrollments)
                .Include(c => c.Feedbacks)
                .FirstOrDefaultAsync(c => c.courseId == courseId);
        }

        public async Task<Course?> GetByTitleAsync(string title)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.title.ToLower() == title.ToLower());
        }

        public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            return await _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Enrollments)
                .Where(c => c.Instructors.Any(i => i.userId == instructorId))
                .OrderByDescending(c => c.createdAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByStatusAsync(CourseStatus status)
        {
            return await _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Enrollments)
                .Where(c => c.status == status)
                .OrderByDescending(c => c.createdAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByLevelAsync(CourseLevel level)
        {
            return await _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Enrollments)
                .Where(c => c.level == level)
                .OrderByDescending(c => c.createdAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByTypeAsync(CourseType courseType)
        {
            return await _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Enrollments)
                .Where(c => c.courseType == courseType)
                .OrderByDescending(c => c.createdAt)
                .ToListAsync();
        }

        public async Task<Pagination<Course>> GetCoursesWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Enrollments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => 
                    c.title.ToLower().Contains(searchTerm.ToLower()) ||
                    c.description.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.createdAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Pagination<Course>
            {
                Items = items,
                TotalItemsCount = totalCount,
                PageIndex = pageNumber - 1, // PageIndex is 0-based, but pageNumber parameter is 1-based
                PageSize = pageSize
            };
        }

        public async Task<bool> IsTitleUniqueAsync(string title, Guid? excludeCourseId = null)
        {
            var query = _dbSet.Where(c => c.title.ToLower() == title.ToLower());
            
            if (excludeCourseId.HasValue)
            {
                query = query.Where(c => c.courseId != excludeCourseId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _dbSet
                .Include(c => c.Instructors)
                .FirstOrDefaultAsync(c => c.courseId == courseId);

            if (course == null)
                throw new ArgumentException($"Course with ID {courseId} not found");

            var instructor = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.userId == instructorId);

            if (instructor == null)
                throw new ArgumentException($"User with ID {instructorId} not found");

            if (!course.Instructors.Any(i => i.userId == instructorId))
            {
                course.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _dbSet
                .Include(c => c.Instructors)
                .FirstOrDefaultAsync(c => c.courseId == courseId);

            if (course == null)
                throw new ArgumentException($"Course with ID {courseId} not found");

            var instructor = course.Instructors.FirstOrDefault(i => i.userId == instructorId);
            if (instructor != null)
            {
                course.Instructors.Remove(instructor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetCourseInstructorsAsync(Guid courseId)
        {
            var course = await _dbSet
                .Include(c => c.Instructors)
                .FirstOrDefaultAsync(c => c.courseId == courseId);

            return course?.Instructors ?? new List<User>();
        }
    }
} 