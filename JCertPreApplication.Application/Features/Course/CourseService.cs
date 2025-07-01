using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Course
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public CourseService(ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            // Check if title is unique
            if (!await _courseRepository.IsTitleUniqueAsync(createCourseDto.Title))
                throw ApiException.BadRequest("COURSE_TITLE_EXISTS", "A course with this title already exists");

            // Create new course entity
            var course = new Domain.Entities.Course
            {
                courseId = Guid.NewGuid(),
                title = createCourseDto.Title,
                description = createCourseDto.Description,
                level = createCourseDto.Level,
                courseType = createCourseDto.CourseType,
                price = createCourseDto.Price,
                thumbnailUrl = createCourseDto.ThumbnailUrl,
                status = CourseStatus.Draft, // Default status
                createdAt = DateTime.UtcNow
            };

            await _courseRepository.InsertAsync(course);

            // Return course directly without querying DB again
            // The course object in memory already contains all necessary data
            return MapToCourseDto(course);
        }

        public async Task<CourseDto> GetCourseByIdAsync(Guid courseId)
        {
            var course = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            return MapToCourseDto(course);
        }

        public async Task<IEnumerable<CourseListDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(MapToCourseListDto);
        }

        public async Task<Pagination<CourseListDto>> GetCoursesWithPaginationAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Limit max page size

            var paginatedCourses = await _courseRepository.GetCoursesWithPaginationAsync(pageNumber, pageSize, searchTerm);
            
            return new Pagination<CourseListDto>
            {
                Items = paginatedCourses.Items.Select(MapToCourseListDto).ToList(),
                TotalItemsCount = paginatedCourses.TotalItemsCount,
                PageIndex = pageNumber - 1, // Convert to 0-based for consistency
                PageSize = paginatedCourses.PageSize
            };
        }

        public async Task<CourseDto> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            // Check title uniqueness if title is being updated
            if (!string.IsNullOrEmpty(updateCourseDto.Title) && course.title != updateCourseDto.Title)
            {
                if (!await _courseRepository.IsTitleUniqueAsync(updateCourseDto.Title, courseId))
                    throw ApiException.BadRequest("COURSE_TITLE_EXISTS", "A course with this title already exists");
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(updateCourseDto.Title))
                course.title = updateCourseDto.Title;
            if (!string.IsNullOrEmpty(updateCourseDto.Description))
                course.description = updateCourseDto.Description;
            if (updateCourseDto.Level.HasValue)
                course.level = updateCourseDto.Level.Value;
            if (updateCourseDto.CourseType.HasValue)
                course.courseType = updateCourseDto.CourseType.Value;
            if (updateCourseDto.Price.HasValue)
                course.price = updateCourseDto.Price.Value;
            if (updateCourseDto.ThumbnailUrl != null) // Allow setting to null
                course.thumbnailUrl = updateCourseDto.ThumbnailUrl;
            if (updateCourseDto.Status.HasValue)
                course.status = updateCourseDto.Status.Value;

            await _courseRepository.UpdateAsync(course);

            var updatedCourse = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            return MapToCourseDto(updatedCourse!);
        }

        public async Task DeleteCourseAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            // Check if course has enrollments
            var courseWithDetails = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            if (courseWithDetails?.Enrollments?.Any() == true)
                throw ApiException.BadRequest("COURSE_HAS_ENROLLMENTS", "Cannot delete course with existing enrollments");

            await _courseRepository.DeleteAsync(course);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            var user = await _userRepository.GetByIdAsync(instructorId);
            if (user == null)
                throw ApiException.NotFound("User", instructorId);

            var courses = await _courseRepository.GetCoursesByInstructorAsync(instructorId);
            return courses.Select(MapToCourseListDto);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByStatusAsync(CourseStatus status)
        {
            var courses = await _courseRepository.GetCoursesByStatusAsync(status);
            return courses.Select(MapToCourseListDto);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByLevelAsync(CourseLevel level)
        {
            var courses = await _courseRepository.GetCoursesByLevelAsync(level);
            return courses.Select(MapToCourseListDto);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByTypeAsync(CourseType courseType)
        {
            var courses = await _courseRepository.GetCoursesByTypeAsync(courseType);
            return courses.Select(MapToCourseListDto);
        }

        public async Task UpdateCourseStatusAsync(Guid courseId, CourseStatus status)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            course.status = status;
            await _courseRepository.UpdateAsync(course);
        }

        public async Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            var user = await _userRepository.GetByIdAsync(instructorId);
            if (user == null)
                throw ApiException.NotFound("User", instructorId);

            await _courseRepository.AddInstructorToCourseAsync(courseId, instructorId);
        }

        public async Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            await _courseRepository.RemoveInstructorFromCourseAsync(courseId, instructorId);
        }

        public async Task<IEnumerable<AppUserDto>> GetCourseInstructorsAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            var instructors = await _courseRepository.GetCourseInstructorsAsync(courseId);
            return instructors.Select(MapToAppUserDto);
        }

        private static CourseDto MapToCourseDto(Domain.Entities.Course course)
        {
            return new CourseDto
            {
                CourseId = course.courseId,
                Title = course.title,
                Description = course.description,
                Level = course.level,
                CourseType = course.courseType,
                Price = course.price,
                ThumbnailUrl = course.thumbnailUrl,
                Status = course.status,
                CreatedAt = course.createdAt,
                LessonsCount = course.Lessons?.Count ?? 0,
                LivestreamsCount = course.Livestreams?.Count ?? 0,
                EnrollmentsCount = course.Enrollments?.Count ?? 0,
                Instructors = course.Instructors?.Select(MapToAppUserDto).ToList() ?? new List<AppUserDto>()
            };
        }

        private static CourseListDto MapToCourseListDto(Domain.Entities.Course course)
        {
            return new CourseListDto
            {
                CourseId = course.courseId,
                Title = course.title,
                Description = course.description,
                Level = course.level,
                CourseType = course.courseType,
                Price = course.price,
                ThumbnailUrl = course.thumbnailUrl,
                Status = course.status,
                CreatedAt = course.createdAt,
                EnrollmentsCount = course.Enrollments?.Count ?? 0,
                InstructorsCount = course.Instructors?.Count ?? 0
            };
        }

        private static AppUserDto MapToAppUserDto(User user)
        {
            return new AppUserDto
            {
                Id = user.userId,
                fullName = user.fullName,
                email = user.email,
                phone = user.phone
            };
        }
    }
} 