using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.Persistence.Repositories
{
    public class CourseInstructorRepository : GenericRepository<CourseInstructor>, ICourseInstructorRepository
    {
        public CourseInstructorRepository(JCertPreDatabaseContext context) : base(context)
        {
        }

        public async Task<CourseInstructor?> GetByIds(Guid courseId, Guid instructorId)
        {
            return await _dbSet
                .Include(ci => ci.Instructor)
                .Include(ci => ci.Course)
                .FirstOrDefaultAsync(ci => ci.CourseId == courseId && ci.InstructorId == instructorId);
        }

        public async Task<IEnumerable<CourseInstructor>> GetActiveByCourseId(Guid courseId)
        {
            return await _dbSet
                .Include(ci => ci.Instructor)
                .Where(ci => ci.CourseId == courseId && ci.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseInstructor>> GetHistoryByCourseId(Guid courseId)
        {
            return await _dbSet
                .Include(ci => ci.Instructor)
                .Where(ci => ci.CourseId == courseId)
                .OrderByDescending(ci => ci.AssignedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseInstructor>> GetActiveByInstructorId(Guid instructorId)
        {
            return await _dbSet
                .Include(ci => ci.Course)
                .Where(ci => ci.InstructorId == instructorId && ci.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseInstructor>> GetHistoryByInstructorId(Guid instructorId)
        {
            return await _dbSet
                .Include(ci => ci.Course)
                .Where(ci => ci.InstructorId == instructorId)
                .OrderByDescending(ci => ci.AssignedOn)
                .ToListAsync();
        }

        public async Task<bool> IsInstructorAssignedToCourse(Guid courseId, Guid instructorId)
        {
            return await _dbSet.AnyAsync(ci => 
                ci.CourseId == courseId && 
                ci.InstructorId == instructorId && 
                ci.IsActive);
        }
    }
} 