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
                .Include(c => c.CourseInstructors)
                    .ThenInclude(ci => ci.Instructor)
                .Include(c => c.Lessons)
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
                .Include(c => c.CourseInstructors)
                    .ThenInclude(ci => ci.Instructor)
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
                query = query.Where(c => c.CourseInstructors.Any(ci => 
                    ci.InstructorId == queryParameters.InstructorId.Value && ci.IsActive));
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
                PageIndex = pageNumber - 1,
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

        public async Task<bool> HasActiveInstructorAsync(Guid courseId, Guid instructorId)
        {
            return await _context.Set<CourseInstructor>()
                .AnyAsync(ci => ci.CourseId == courseId && 
                               ci.InstructorId == instructorId && 
                               ci.IsActive);
        }

        public async Task<CourseInstructor> AddInstructorToCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _dbSet.FindAsync(courseId)
                ?? throw new ArgumentException($"Course with ID {courseId} not found");

            var instructor = await _context.Set<User>().FindAsync(instructorId)
                ?? throw new ArgumentException($"User with ID {instructorId} not found");

            var courseInstructor = new CourseInstructor
            {
                CourseId = courseId,
                InstructorId = instructorId,
                AssignedOn = DateTime.UtcNow,
                IsActive = true
            };

            await _context.Set<CourseInstructor>().AddAsync(courseInstructor);
            return courseInstructor;
        }

        public async Task<bool> DeactivateInstructorFromCourseAsync(Guid courseId, Guid instructorId, string? notes = null)
        {
            var courseInstructor = await _context.Set<CourseInstructor>()
                .FirstOrDefaultAsync(ci => ci.CourseId == courseId && 
                                         ci.InstructorId == instructorId && 
                                         ci.IsActive);

            if (courseInstructor == null)
                return false;

            courseInstructor.IsActive = false;
            courseInstructor.LeftOn = DateTime.UtcNow;
            courseInstructor.Notes = notes;

            return true;
        }

        public async Task<IEnumerable<User>> GetActiveCourseInstructorsAsync(Guid courseId)
        {
            var instructors = await _context.Set<CourseInstructor>()
                .Include(ci => ci.Instructor)
                    .ThenInclude(u => u.Role)
                .Where(ci => ci.CourseId == courseId && ci.IsActive)
                .Select(ci => ci.Instructor)
                .ToListAsync();

            return instructors;
        }

        public async Task<IEnumerable<CourseInstructor>> GetCourseInstructorHistoryAsync(Guid courseId)
        {
            return await _context.Set<CourseInstructor>()
                .Include(ci => ci.Instructor)
                .Where(ci => ci.CourseId == courseId)
                .OrderByDescending(ci => ci.AssignedOn)
                .ToListAsync();
        }
    }
} 