using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course?> GetCourseWithDetailsAsync(Guid courseId);
        Task<Course?> GetByTitleAsync(string title);
        Task<Pagination<Course>> GetCoursesWithPaginationAsync(CourseQueryParameters queryParameters);
        Task<bool> IsTitleUniqueAsync(string title, Guid? excludeCourseId = null);
        Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId);
        Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId);
        Task<IEnumerable<User>> GetCourseInstructorsAsync(Guid courseId);
    }
} 