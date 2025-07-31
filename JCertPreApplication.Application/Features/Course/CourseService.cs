using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Features.Course
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileService _fileService;

        public CourseService(ICourseRepository courseRepository, IUserRepository userRepository, IFileService fileService)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _fileService = fileService;
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            try
            {
                // Check if title is unique
                if (!await _courseRepository.IsTitleUniqueAsync(createCourseDto.Title))
                    throw ApiException.BadRequest("COURSE_TITLE_EXISTS", "A course with this title already exists");

                // Generate course ID first
                var courseId = Guid.NewGuid();
                string? thumbnailUrl = null;

                // Handle thumbnail upload if provided
                if (createCourseDto.ThumbnailFile != null)
                {
                    // Create a custom FormFile with courseId as filename
                    var customFormFile = CreateCustomFormFile(createCourseDto.ThumbnailFile, courseId.ToString());
                    
                    // Upload thumbnail to file service
                    var uploadResult = await _fileService.UploadImageAsync(customFormFile);
                    
                    if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                    {
                        thumbnailUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                // Create new course entity
                var course = new Domain.Entities.Course
                {
                    courseId = courseId,
                    title = createCourseDto.Title,
                    description = createCourseDto.Description,
                    level = createCourseDto.Level,
                    courseType = createCourseDto.CourseType,
                    price = createCourseDto.Price,
                    thumbnailUrl = thumbnailUrl,
                    status = CourseStatus.Draft, // Default status
                    createdAt = DateTime.UtcNow
                };

                await _courseRepository.InsertAsync(course);
                await _courseRepository.SaveChangesAsync();

                // Return course directly without querying DB again
                // The course object in memory already contains all necessary data
                return MapToCourseDto(course);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("COURSE_CREATE_ERROR", $"An error occurred while creating the course: {ex.Message}");
            }
        }

        public async Task<CourseDto> GetCourseByIdAsync(Guid courseId)
        {
            var course = await _courseRepository.GetCourseWithDetailsAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            return MapToCourseDto(course);
        }

        public async Task<Pagination<CourseListDto>> GetCoursesWithPaginationAsync(CourseQueryParameters queryParameters)
        {
            var paginatedCourses = await _courseRepository.GetCoursesWithPaginationAsync(queryParameters);
            
            return new Pagination<CourseListDto>
            {
                Items = paginatedCourses.Items.Select(MapToCourseListDto).ToList(),
                TotalItemsCount = paginatedCourses.TotalItemsCount,
                PageIndex = paginatedCourses.PageIndex,
                PageSize = paginatedCourses.PageSize
            };
        }

        public async Task<CourseDto> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateCourseDto)
        {
            try
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

                // Handle thumbnail upload if new file is provided
                if (updateCourseDto.ThumbnailFile != null)
                {
                    // Delete old thumbnail from Cloudinary if exists
                    if (!string.IsNullOrEmpty(course.thumbnailUrl))
                    {
                        try
                        {
                            var oldPublicId = ExtractCloudinaryPublicId(course.thumbnailUrl);
                            if (!string.IsNullOrWhiteSpace(oldPublicId))
                            {
                                await _fileService.DeleteImageAsync(oldPublicId);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log warning but don't fail the update if old image deletion fails
                            System.Diagnostics.Debug.WriteLine($"Warning: Failed to delete old Cloudinary thumbnail: {ex.Message}");
                        }
                    }

                    // Upload new thumbnail using courseId as filename
                    var customFormFile = CreateCustomFormFile(updateCourseDto.ThumbnailFile, courseId.ToString());
                    var uploadResult = await _fileService.UploadImageAsync(customFormFile);
                    
                    if (uploadResult != null && !string.IsNullOrEmpty(uploadResult.SecureUrl?.ToString()))
                    {
                        course.thumbnailUrl = uploadResult.SecureUrl.ToString();
                    }
                }
                else if (updateCourseDto.ThumbnailUrl != null) // Allow setting to null or updating URL
                {
                    course.thumbnailUrl = updateCourseDto.ThumbnailUrl;
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
                if (updateCourseDto.Status.HasValue)
                    course.status = updateCourseDto.Status.Value;

                await _courseRepository.UpdateAsync(course);
                await _courseRepository.SaveChangesAsync();

                var updatedCourse = await _courseRepository.GetCourseWithDetailsAsync(courseId);
                return MapToCourseDto(updatedCourse!);
            }
            catch (ApiException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("COURSE_UPDATE_ERROR", $"An error occurred while updating the course: {ex.Message}");
            }
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
            await _courseRepository.SaveChangesAsync();
        }

        public async Task UpdateCourseStatusAsync(Guid courseId, CourseStatus status)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            course.status = status;
            await _courseRepository.UpdateAsync(course);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task AddInstructorToCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            var user = await _userRepository.GetByIdAsync(instructorId);
            if (user == null)
                throw ApiException.NotFound("User", instructorId);

            // Check if instructor is already active in this course
            if (await _courseRepository.HasActiveInstructorAsync(courseId, instructorId))
                throw ApiException.BadRequest("INSTRUCTOR_ALREADY_ACTIVE", "This instructor is already active in this course");

            // Check if course already has any active instructor
            var activeInstructors = await _courseRepository.GetActiveCourseInstructorsAsync(courseId);
            if (activeInstructors.Any())
                throw ApiException.BadRequest("COURSE_HAS_ACTIVE_INSTRUCTOR", "Course already has an active instructor. Please remove the current instructor before assigning a new one.");

            await _courseRepository.AddInstructorToCourseAsync(courseId, instructorId);
            await _courseRepository.SaveChangesAsync();
        }

        public async Task RemoveInstructorFromCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            // Try to deactivate the instructor
            var deactivated = await _courseRepository.DeactivateInstructorFromCourseAsync(courseId, instructorId);
            if (!deactivated)
                throw ApiException.BadRequest("INSTRUCTOR_NOT_ACTIVE", "This instructor is not active in this course");

            await _courseRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppUserDto>> GetCourseInstructorsAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            var instructors = await _courseRepository.GetActiveCourseInstructorsAsync(courseId);
            return instructors.Select(MapToAppUserDto);
        }

        public async Task<IEnumerable<CourseInstructorHistoryDto>> GetCourseInstructorHistoryAsync(Guid courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw ApiException.NotFound("Course", courseId);

            var history = await _courseRepository.GetCourseInstructorHistoryAsync(courseId);
            return history.Select(h => new CourseInstructorHistoryDto
            {
                CourseId = h.CourseId,
                InstructorId = h.InstructorId,
                InstructorName = h.Instructor.fullName,
                AssignedOn = h.AssignedOn,
                LeftOn = h.LeftOn,
                IsActive = h.IsActive,
                Notes = h.Notes
            });
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            var user = await _userRepository.GetByIdAsync(instructorId);
            if (user == null)
                throw ApiException.NotFound("User", instructorId);

            var courses = await _courseRepository.GetCoursesByInstructorAsync(instructorId);
            return courses.Select(MapToCourseListDto);
        }

        public async Task<IEnumerable<CourseListDto>> GetCoursesByStudentAsync(Guid studentId)
        {
            var user = await _userRepository.GetByIdAsync(studentId);
            if (user == null)
                throw ApiException.NotFound("User", studentId);

            var courses = await _courseRepository.GetCoursesByStudentAsync(studentId);
            return courses.Select(MapToCourseListDto);
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
                EnrollmentsCount = course.Enrollments?.Count ?? 0,
                Instructors = course.CourseInstructors?
                    .Where(ci => ci.IsActive)
                    .Select(ci => MapToAppUserDto(ci.Instructor))
                    .ToList() ?? new List<AppUserDto>()
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
                InstructorsCount = course.CourseInstructors?.Count(ci => ci.IsActive) ?? 0
            };
        }

        private static AppUserDto MapToAppUserDto(User user)
        {
            return new AppUserDto
            {
                Id = user.userId,
                fullName = user.fullName,
                email = user.email,
                phone = user.phone,
                avatarUrl = user.avatarUrl,
                credit = user.credit,
                createdAt = user.createdAt,
                lastLogin = user.lastLogin,
                status = user.status,
                roleId = user.roleId,
                roleName = user.Role?.roleName
            };
        }

        private static IFormFile CreateCustomFormFile(IFormFile originalFile, string customFileName)
        {
            // Get the file extension from original file
            var extension = Path.GetExtension(originalFile.FileName);
            var newFileName = customFileName + extension;

            return new CustomFormFile(originalFile, newFileName);
        }

        private static string? ExtractCloudinaryPublicId(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var uploadIdx = Array.IndexOf(segments, "upload");

                if (uploadIdx == -1 || uploadIdx >= segments.Length - 1)
                    return null;

                // Get everything after upload/ as potential public ID
                var publicIdParts = segments.Skip(uploadIdx + 1).ToArray();

                // Skip version if present (starts with 'v' followed by numbers)
                if (publicIdParts.Length > 0 && publicIdParts[0].StartsWith("v") &&
                    publicIdParts[0].Length > 1 && publicIdParts[0].Skip(1).All(char.IsDigit))
                {
                    publicIdParts = publicIdParts.Skip(1).ToArray();
                }

                if (publicIdParts.Length == 0) return null;

                // Remove file extension from the last part
                var lastPart = publicIdParts.Last();
                var dotIndex = lastPart.LastIndexOf('.');
                if (dotIndex > 0)
                {
                    publicIdParts[publicIdParts.Length - 1] = lastPart.Substring(0, dotIndex);
                }
                
                return string.Join("/", publicIdParts);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Custom IFormFile implementation to override filename while preserving original file content
    /// </summary>
    internal class CustomFormFile : IFormFile
    {
        private readonly IFormFile _originalFile;
        private readonly string _customFileName;

        public CustomFormFile(IFormFile originalFile, string customFileName)
        {
            _originalFile = originalFile;
            _customFileName = customFileName;
        }

        public string ContentType => _originalFile.ContentType;
        public string ContentDisposition => _originalFile.ContentDisposition;
        public IHeaderDictionary Headers => _originalFile.Headers;
        public long Length => _originalFile.Length;
        public string Name => _originalFile.Name;
        public string FileName => _customFileName; // This is the overridden filename

        public void CopyTo(Stream target) => _originalFile.CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
            _originalFile.CopyToAsync(target, cancellationToken);
        public Stream OpenReadStream() => _originalFile.OpenReadStream();
    }
} 