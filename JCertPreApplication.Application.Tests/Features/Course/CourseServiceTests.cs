using Xunit;
using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Utilities;

namespace JCertPreApplication.Application.Tests.Features.Course
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _courseService = new CourseService(_mockCourseRepository.Object, _mockUserRepository.Object);
        }

        #region CreateCourseAsync Tests

        [Fact]
        public async Task CreateCourseAsync_Should_ReturnCourseDto_When_TitleIsUnique()
        {
            // Arrange
            var createDto = new CreateCourseDto
            {
                Title = "Test Course",
                Description = "Test Description",
                Level = CourseLevel.Beginner,
                CourseType = CourseType.SelfPaced,
                Price = 100,
                ThumbnailUrl = "http://example.com/thumbnail.jpg"
            };

            _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _courseService.CreateCourseAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Title, result.Title);
            Assert.Equal(createDto.Description, result.Description);
            Assert.Equal(createDto.Level, result.Level);
            Assert.Equal(createDto.CourseType, result.CourseType);
            Assert.Equal(createDto.Price, result.Price);
            Assert.Equal(createDto.ThumbnailUrl, result.ThumbnailUrl);
            Assert.Equal(CourseStatus.Draft, result.Status);

            _mockCourseRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Course>()), Times.Once);
            _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCourseAsync_Should_ThrowApiException_When_TitleIsNotUnique()
        {
            // Arrange
            var createDto = new CreateCourseDto { Title = "Existing Course" };

            _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _courseService.CreateCourseAsync(createDto));

            Assert.Equal("COURSE_TITLE_EXISTS", exception.ErrorCode);
        }

        #endregion

        #region GetCourseByIdAsync Tests

        [Fact]
        public async Task GetCourseByIdAsync_Should_ReturnCourseDto_When_CourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course
            {
                courseId = courseId,
                title = "Test Course",
                status = CourseStatus.Draft
            };

            _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.GetCourseByIdAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(courseId, result.CourseId);
            Assert.Equal(course.title, result.Title);
        }

        [Fact]
        public async Task GetCourseByIdAsync_Should_ThrowApiException_When_CourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync((Domain.Entities.Course)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _courseService.GetCourseByIdAsync(courseId));

            Assert.Contains("Course", exception.Message);
        }

        #endregion

        #region GetCoursesWithPaginationAsync Tests

        [Fact]
        public async Task GetCoursesWithPaginationAsync_Should_ReturnPaginatedCourseListDto_When_Called()
        {
            // Arrange
            var queryParameters = new CourseQueryParameters { PageIndex = 1, PageSize = 10 };
            var courses = new List<Domain.Entities.Course>
            {
                new() { courseId = Guid.NewGuid(), title = "Course 1" },
                new() { courseId = Guid.NewGuid(), title = "Course 2" }
            };

            var paginatedCourses = new Pagination<Domain.Entities.Course>
            {
                Items = courses,
                TotalItemsCount = 2,
                PageIndex = 1,
                PageSize = 10
            };

            _mockCourseRepository.Setup(x => x.GetCoursesWithPaginationAsync(queryParameters))
                .ReturnsAsync(paginatedCourses);

            // Act
            var result = await _courseService.GetCoursesWithPaginationAsync(queryParameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.TotalItemsCount);
            Assert.Equal(1, result.PageIndex);
            Assert.Equal(10, result.PageSize);
        }

        #endregion

        #region UpdateCourseAsync Tests

        [Fact]
        public async Task UpdateCourseAsync_Should_ReturnUpdatedCourseDto_When_CourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course
            {
                courseId = courseId,
                title = "Original Title"
            };

            var updateDto = new UpdateCourseDto
            {
                Title = "Updated Title",
                Description = "Updated Description"
            };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(updateDto.Title, courseId))
                .ReturnsAsync(true);
            _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.UpdateCourseAsync(courseId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Title, result.Title);
            Assert.Equal(updateDto.Description, result.Description);

            _mockCourseRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Course>()), Times.Once);
            _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCourseAsync_Should_ThrowApiException_When_CourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var updateDto = new UpdateCourseDto { Title = "Updated Title" };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync((Domain.Entities.Course)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _courseService.UpdateCourseAsync(courseId, updateDto));

            Assert.Contains("Course", exception.Message);
        }

        [Fact]
        public async Task UpdateCourseAsync_Should_ThrowApiException_When_NewTitleIsNotUnique()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course
            {
                courseId = courseId,
                title = "Original Title"
            };

            var updateDto = new UpdateCourseDto { Title = "Duplicate Title" };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(updateDto.Title, courseId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _courseService.UpdateCourseAsync(courseId, updateDto));

            Assert.Equal("COURSE_TITLE_EXISTS", exception.ErrorCode);
        }

        #endregion

        #region DeleteCourseAsync Tests

        [Fact]
        public async Task DeleteCourseAsync_Should_CompleteSuccessfully_When_CourseHasNoEnrollments()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course
            {
                courseId = courseId,
                title = "Test Course",
                Enrollments = new List<Enrollment>()
            };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            await _courseService.DeleteCourseAsync(courseId);

            // Assert
            _mockCourseRepository.Verify(x => x.DeleteAsync(course), Times.Once);
            _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_Should_ThrowApiException_When_CourseHasEnrollments()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course
            {
                courseId = courseId,
                title = "Test Course",
                Enrollments = new List<Enrollment> { new Enrollment() }
            };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _courseService.DeleteCourseAsync(courseId));

            Assert.Equal("COURSE_HAS_ENROLLMENTS", exception.ErrorCode);
        }

        #endregion

        #region UpdateCourseStatusAsync Tests

        [Fact]
        public async Task UpdateCourseStatusAsync_Should_UpdateStatus_When_CourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course
            {
                courseId = courseId,
                status = CourseStatus.Draft
            };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            await _courseService.UpdateCourseStatusAsync(courseId, CourseStatus.Published);

            // Assert
            Assert.Equal(CourseStatus.Published, course.status);
            _mockCourseRepository.Verify(x => x.UpdateAsync(course), Times.Once);
            _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCourseStatusAsync_Should_ThrowApiException_When_CourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync((Domain.Entities.Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(
                () => _courseService.UpdateCourseStatusAsync(courseId, CourseStatus.Published));
        }

        #endregion

        #region AddInstructorToCourseAsync Tests

        [Fact]
        public async Task AddInstructorToCourseAsync_Should_Succeed_When_CourseAndUserExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var course = new Domain.Entities.Course { courseId = courseId };
            var instructor = new User { userId = instructorId };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                .ReturnsAsync(instructor);

            // Act
            await _courseService.AddInstructorToCourseAsync(courseId, instructorId);

            // Assert
            _mockCourseRepository.Verify(x => x.AddInstructorToCourseAsync(courseId, instructorId), Times.Once);
            _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddInstructorToCourseAsync_Should_ThrowApiException_When_CourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync((Domain.Entities.Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(
                () => _courseService.AddInstructorToCourseAsync(courseId, instructorId));
        }

        [Fact]
        public async Task AddInstructorToCourseAsync_Should_ThrowApiException_When_UserDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var course = new Domain.Entities.Course { courseId = courseId };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(
                () => _courseService.AddInstructorToCourseAsync(courseId, instructorId));
        }

        #endregion

        #region RemoveInstructorFromCourseAsync Tests

        [Fact]
        public async Task RemoveInstructorFromCourseAsync_Should_Succeed_When_CourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var course = new Domain.Entities.Course { courseId = courseId };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            // Act
            await _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId);

            // Assert
            _mockCourseRepository.Verify(x => x.RemoveInstructorFromCourseAsync(courseId, instructorId), Times.Once);
            _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveInstructorFromCourseAsync_Should_ThrowApiException_When_CourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync((Domain.Entities.Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(
                () => _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId));
        }

        #endregion

        #region GetCourseInstructorsAsync Tests

        [Fact]
        public async Task GetCourseInstructorsAsync_Should_ReturnInstructors_When_CourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Domain.Entities.Course { courseId = courseId };
            var instructors = new List<User>
            {
                new() { userId = Guid.NewGuid(), fullName = "Instructor 1" },
                new() { userId = Guid.NewGuid(), fullName = "Instructor 2" }
            };

            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync(course);
            _mockCourseRepository.Setup(x => x.GetCourseInstructorsAsync(courseId))
                .ReturnsAsync(instructors);

            // Act
            var result = await _courseService.GetCourseInstructorsAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(instructors[0].fullName, result.First().fullName);
        }

        [Fact]
        public async Task GetCourseInstructorsAsync_Should_ThrowApiException_When_CourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                .ReturnsAsync((Domain.Entities.Course)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(
                () => _courseService.GetCourseInstructorsAsync(courseId));
        }

        #endregion
    }
}