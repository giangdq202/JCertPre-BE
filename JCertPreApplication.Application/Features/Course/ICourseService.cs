using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Course
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId);
        Task<IEnumerable<CourseListDto>> GetAllCoursesAsync();
        Task<Pagination<CourseListDto>> GetCoursesWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<CourseDto> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto);
        Task DeleteCourseAsync(Guid courseId);
        Task<IEnumerable<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<IEnumerable<CourseListDto>> GetCoursesByStatusAsync(CourseStatus status);
        Task<IEnumerable<CourseListDto>> GetCoursesByLevelAsync(CourseLevel level);
        Task<IEnumerable<CourseListDto>> GetCoursesByTypeAsync(CourseType courseType);
        Task UpdateCourseStatusAsync(Guid courseId, CourseStatus status);
        Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId);
        Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId);
        Task<IEnumerable<AppUserDto>> GetCourseInstructorsAsync(Guid courseId);
    }
} 