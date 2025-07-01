using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Application.Dtos.Course;
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

        public async Task<Pagination<Course>> GetCoursesWithPaginationAsync(CourseQueryParameters queryParameters)
        {
            var query = _dbSet
                .Include(c => c.Instructors)
                .Include(c => c.Enrollments)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
            {
                query = query.Where(c => 
                    c.title.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                    c.description.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            // Apply instructor filter
            if (queryParameters.InstructorId.HasValue)
            {
                query = query.Where(c => c.Instructors.Any(i => i.userId == queryParameters.InstructorId.Value));
            }

            // Apply status filter
            if (queryParameters.Status.HasValue)
            {
                query = query.Where(c => c.status == queryParameters.Status.Value);
            }

            // Apply level filter
            if (queryParameters.Level.HasValue)
            {
                query = query.Where(c => c.level == queryParameters.Level.Value);
            }

            // Apply course type filter
            if (queryParameters.CourseType.HasValue)
            {
                query = query.Where(c => c.courseType == queryParameters.CourseType.Value);
            }

            // Validate and set pagination parameters
            var pageNumber = queryParameters.PageNumber <= 0 ? 1 : queryParameters.PageNumber;
            var pageSize = queryParameters.PageSize <= 0 ? 10 : (queryParameters.PageSize > 100 ? 100 : queryParameters.PageSize);

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
                // Commit sẽ được thực hiện ở Service layer
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
                // Commit sẽ được thực hiện ở Service layer
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