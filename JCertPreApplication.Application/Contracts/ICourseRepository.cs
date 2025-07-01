using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Utilities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
        Task<Course?> GetByTitleAsync(string title);
        Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<IEnumerable<Course>> GetCoursesByStatusAsync(CourseStatus status);
        Task<IEnumerable<Course>> GetCoursesByLevelAsync(CourseLevel level);
        Task<IEnumerable<Course>> GetCoursesByTypeAsync(CourseType courseType);
        Task<Pagination<Course>> GetCoursesWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<bool> IsTitleUniqueAsync(string title, Guid? excludeCourseId = null);
        Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId);
        Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId);
        Task<IEnumerable<User>> GetCourseInstructorsAsync(Guid courseId);
    }
} 