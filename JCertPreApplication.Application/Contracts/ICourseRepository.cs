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
        Task<bool> HasActiveInstructorAsync(Guid courseId, Guid instructorId);
        Task<CourseInstructor> AddInstructorToCourseAsync(Guid courseId, Guid instructorId);
        Task<bool> DeactivateInstructorFromCourseAsync(Guid courseId, Guid instructorId, string? notes = null);
        Task<IEnumerable<User>> GetActiveCourseInstructorsAsync(Guid courseId);
        Task<IEnumerable<CourseInstructor>> GetCourseInstructorHistoryAsync(Guid courseId);
        Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<IEnumerable<Course>> GetCoursesByStudentAsync(Guid studentId);
    }
} 