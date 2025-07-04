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
        Task<Pagination<CourseListDto>> GetCoursesWithPaginationAsync(CourseQueryParameters queryParameters);
        Task<CourseDto> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto);
        Task DeleteCourseAsync(Guid courseId);
        Task UpdateCourseStatusAsync(Guid courseId, CourseStatus status);
        Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId);
        Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId);
        Task<IEnumerable<AppUserDto>> GetCourseInstructorsAsync(Guid courseId);
        Task<IEnumerable<CourseInstructorHistoryDto>> GetCourseInstructorHistoryAsync(Guid courseId);
    }
} 