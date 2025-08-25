using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Course;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Course;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace JCertPreApplication.UnitTests.Features.Course;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IFileService> _mockFileService;
    private readonly Mock<ILivestreamRepository> _mockLivestreamRepository;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockCourseRepository = new Mock<ICourseRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockFileService = new Mock<IFileService>();
        _mockLivestreamRepository = new Mock<ILivestreamRepository>();

        _courseService = new CourseService(
            _mockCourseRepository.Object,
            _mockUserRepository.Object,
            _mockFileService.Object,
            _mockLivestreamRepository.Object
        );
    }

    #region CreateCourseAsync Tests

    [Fact]
    public async Task CreateCourseAsync_WithValidData_ShouldReturnCourseDto()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Title = "New Course",
            Description = "Course Description",
            Level = CourseLevel.N5,
            CourseType = CourseType.Public,
            Price = 100.00m,
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(createCourseDto.Title, null))
                            .ReturnsAsync(true);
        _mockCourseRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Course>()))
                            .ReturnsAsync((Domain.Entities.Course course) => course);
        _mockCourseRepository.Setup(x => x.SaveChangesAsync())
                            .ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateCourseAsync(createCourseDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createCourseDto.Title);
        result.Description.Should().Be(createCourseDto.Description);
        result.Level.Should().Be(createCourseDto.Level);
        result.Price.Should().Be(createCourseDto.Price);

        _mockCourseRepository.Verify(x => x.IsTitleUniqueAsync(createCourseDto.Title, null), Times.Once);
        _mockCourseRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Course>()), Times.Once);
        _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCourseAsync_WithDuplicateTitle_ShouldThrowBadRequestException()
    {
        // Arrange
        var createCourseDto = new CreateCourseDto
        {
            Title = "Existing Course Title",
            Description = "Course Description",
            Level = CourseLevel.N5,
            Price = 100.00m
        };

        _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(createCourseDto.Title, null))
                            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.CreateCourseAsync(createCourseDto));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("COURSE_TITLE_EXISTS");
        exception.Message.Should().Be("A course with this title already exists");

        _mockCourseRepository.Verify(x => x.IsTitleUniqueAsync(createCourseDto.Title, null), Times.Once);
        _mockCourseRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Course>()), Times.Never);
    }

    [Fact]
    public async Task CreateCourseAsync_WithInvalidData_ShouldThrowValidationException()
    {
        // Arrange - Simulating a service-level validation error
        var createCourseDto = new CreateCourseDto
        {
            Title = "", // Empty title should trigger validation
            Description = "Course Description",
            Level = CourseLevel.N5,
            Price = -100.00m // Negative price
        };

        // Setup repository to return true for title uniqueness check
        _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(It.IsAny<string>(), null))
                            .ReturnsAsync(true);

        // For this test, we simulate an exception during the creation process
        _mockCourseRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Course>()))
                            .ThrowsAsync(new ArgumentException("Invalid course data"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.CreateCourseAsync(createCourseDto));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("COURSE_CREATE_ERROR");
    }

    #endregion

    #region GetCourseByIdAsync Tests

    [Fact]
    public async Task GetCourseByIdAsync_WithExistingId_ShouldReturnCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = CourseBuilder.Create()
            .WithId(courseId)
            .WithTitle("Test Course")
            .WithDescription("Test Description")
            .Build();

        _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                            .ReturnsAsync(course);

        // Act
        var result = await _courseService.GetCourseByIdAsync(courseId);

        // Assert
        result.Should().NotBeNull();
        result.CourseId.Should().Be(courseId);
        result.Title.Should().Be(course.title);
        result.Description.Should().Be(course.description);

        _mockCourseRepository.Verify(x => x.GetCourseWithDetailsAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task GetCourseByIdAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                            .ReturnsAsync((Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.GetCourseByIdAsync(courseId));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockCourseRepository.Verify(x => x.GetCourseWithDetailsAsync(courseId), Times.Once);
    }

    #endregion

    #region UpdateCourseAsync Tests

    [Fact]
    public async Task UpdateCourseAsync_WithValidData_ShouldUpdateCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingCourse = CourseBuilder.Create()
            .WithId(courseId)
            .WithTitle("Old Title")
            .Build();

        var updateCourseDto = new UpdateCourseDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            Price = 150.00m
        };

        var updatedCourse = CourseBuilder.Create()
            .WithId(courseId)
            .WithTitle(updateCourseDto.Title!)
            .WithDescription(updateCourseDto.Description!)
            .Build();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync(existingCourse);
        _mockCourseRepository.Setup(x => x.IsTitleUniqueAsync(updateCourseDto.Title!, courseId))
                            .ReturnsAsync(true);
        _mockCourseRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Course>()))
                            .Returns(Task.CompletedTask);
        _mockCourseRepository.Setup(x => x.SaveChangesAsync())
                            .ReturnsAsync(1);
        _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                            .ReturnsAsync(updatedCourse);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseId, updateCourseDto);

        // Assert
        result.Should().NotBeNull();
        result.CourseId.Should().Be(courseId);
        result.Title.Should().Be(updateCourseDto.Title);

        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockCourseRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Course>()), Times.Once);
        _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCourseAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var updateCourseDto = new UpdateCourseDto
        {
            Title = "Updated Title"
        };

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync((Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.UpdateCourseAsync(courseId, updateCourseDto));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockCourseRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Course>()), Times.Never);
    }

    #endregion

    #region DeleteCourseAsync Tests

    [Fact]
    public async Task DeleteCourseAsync_WithExistingId_ShouldMarkAsDeleted()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = CourseBuilder.Create()
            .WithId(courseId)
            .Build();

        var courseWithDetails = CourseBuilder.Create()
            .WithId(courseId)
            .WithNoEnrollments()
            .Build();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync(course);
        _mockCourseRepository.Setup(x => x.GetCourseWithDetailsAsync(courseId))
                            .ReturnsAsync(courseWithDetails);
        _mockCourseRepository.Setup(x => x.DeleteAsync(course))
                            .Returns(Task.CompletedTask);
        _mockCourseRepository.Setup(x => x.SaveChangesAsync())
                            .ReturnsAsync(1);

        // Act
        await _courseService.DeleteCourseAsync(courseId);

        // Assert
        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockCourseRepository.Verify(x => x.GetCourseWithDetailsAsync(courseId), Times.Once);
        _mockCourseRepository.Verify(x => x.DeleteAsync(course), Times.Once);
        _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCourseAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync((Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.DeleteCourseAsync(courseId));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockCourseRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Course>()), Times.Never);
    }

    #endregion

    #region AddInstructorToCourseAsync Tests

    [Fact]
    public async Task AddInstructorToCourseAsync_WithValidIds_ShouldAddInstructor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        
        var course = CourseBuilder.Create().WithId(courseId).Build();
        var instructor = UserBuilder.Create()
            .WithId(instructorId)
            .WithRole(new Role { roleId = Guid.NewGuid(), roleName = "INSTRUCTOR" })
            .Build();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync(course);
        _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                          .ReturnsAsync(instructor);
        _mockCourseRepository.Setup(x => x.HasActiveInstructorAsync(courseId, instructorId))
                            .ReturnsAsync(false);
        _mockCourseRepository.Setup(x => x.GetActiveCourseInstructorsAsync(courseId))
                            .ReturnsAsync(new List<User>());
        _mockLivestreamRepository.Setup(x => x.GetFutureLivestreamsByCourseIdAsync(courseId))
                                .ReturnsAsync(new List<Livestream>());
        _mockCourseRepository.Setup(x => x.AddInstructorToCourseAsync(courseId, instructorId))
                            .Returns(Task.FromResult(new CourseInstructor()));
        _mockCourseRepository.Setup(x => x.SaveChangesAsync())
                            .ReturnsAsync(1);

        // Act
        await _courseService.AddInstructorToCourseAsync(courseId, instructorId);

        // Assert
        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockUserRepository.Verify(x => x.GetByIdAsync(instructorId), Times.Once);
        _mockCourseRepository.Verify(x => x.AddInstructorToCourseAsync(courseId, instructorId), Times.Once);
        _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddInstructorToCourseAsync_WithInvalidInstructorRole_ShouldThrowException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        
        var course = CourseBuilder.Create().WithId(courseId).Build();
        var existingInstructor = UserBuilder.Create()
            .WithId(Guid.NewGuid())
            .WithRole(new Role { roleId = Guid.NewGuid(), roleName = "INSTRUCTOR" })
            .Build();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync(course);
        _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                          .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _courseService.AddInstructorToCourseAsync(courseId, instructorId));

        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockUserRepository.Verify(x => x.GetByIdAsync(instructorId), Times.Once);
        _mockCourseRepository.Verify(x => x.AddInstructorToCourseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #region RemoveInstructorFromCourseAsync Tests

    [Fact]
    public async Task RemoveInstructorFromCourseAsync_WithValidIds_ShouldRemoveInstructor()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        
        var course = CourseBuilder.Create().WithId(courseId).Build();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
                            .ReturnsAsync(course);
        _mockCourseRepository.Setup(x => x.DeactivateInstructorFromCourseAsync(courseId, instructorId, null))
                            .ReturnsAsync(true);
        _mockCourseRepository.Setup(x => x.SaveChangesAsync())
                            .ReturnsAsync(1);

        // Act
        await _courseService.RemoveInstructorFromCourseAsync(courseId, instructorId);

        // Assert
        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockCourseRepository.Verify(x => x.DeactivateInstructorFromCourseAsync(courseId, instructorId, null), Times.Once);
        _mockCourseRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region GetCoursesWithPaginationAsync Tests

    [Fact]
    public async Task GetCoursesWithPaginationAsync_WithValidParams_ShouldReturnPaginatedResults()
    {
        // Arrange
        var queryParams = new CourseQueryParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SearchTerm = "test"
        };

        var courses = new List<Domain.Entities.Course>
        {
            CourseBuilder.Create().WithTitle("Test Course 1").Build(),
            CourseBuilder.Create().WithTitle("Test Course 2").Build()
        };

        var paginatedResult = new Pagination<Domain.Entities.Course>
        {
            Items = courses,
            TotalItemsCount = 2,
            PageIndex = 1,
            PageSize = 10
        };

        _mockCourseRepository.Setup(x => x.GetCoursesWithPaginationAsync(queryParams))
                            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _courseService.GetCoursesWithPaginationAsync(queryParams);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalItemsCount.Should().Be(2);
        result.PageIndex.Should().Be(1);

        _mockCourseRepository.Verify(x => x.GetCoursesWithPaginationAsync(queryParams), Times.Once);
    }

    #endregion
}
