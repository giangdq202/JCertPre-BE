using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Course
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto, Guid staffUserId);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId);
        Task<IEnumerable<CourseListDto>> GetAllCoursesAsync();
        Task<Pagination<CourseListDto>> GetCoursesWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<CourseDto> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto);
        Task DeleteCourseAsync(Guid courseId);
        Task<IEnumerable<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<IEnumerable<CourseListDto>> GetCoursesByStatusAsync(string status);
        Task<IEnumerable<CourseListDto>> GetCoursesByLevelAsync(CourseLevel level);
        Task<IEnumerable<CourseListDto>> GetCoursesByTypeAsync(CourseType courseType);
        Task UpdateCourseStatusAsync(Guid courseId, string status);
    }
} 