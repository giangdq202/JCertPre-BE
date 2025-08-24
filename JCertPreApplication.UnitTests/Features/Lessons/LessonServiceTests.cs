using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Lesson;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Lessons;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace JCertPreApplication.UnitTests.Features.Lessons;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _mockLessonRepository;
    private readonly Mock<ICourseRepository> _mockCourseRepository;
    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        _mockLessonRepository = new Mock<ILessonRepository>();
        _mockCourseRepository = new Mock<ICourseRepository>();
        _lessonService = new LessonService(_mockLessonRepository.Object, _mockCourseRepository.Object);
    }

    #region GetPaginatedAsync Tests

    [Fact]
    public async Task GetPaginatedAsync_WithValidCourseId_ShouldReturnPaginatedLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 10;
        var searchTerm = "";
        var totalCount = 5;

        var course = LessonServiceFixture.CreateTestCourse();
        var lessons = LessonServiceFixture.CreateLessonList(courseId, totalCount);
        var expectedPagination = LessonServiceFixture.CreatePaginatedLessons(lessons, pageIndex, pageSize, totalCount);

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockLessonRepository.Setup(x => x.GetPaginatedLessonsByCourseAsync(
            It.IsAny<Guid>(),
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _lessonService.GetPaginatedAsync(courseId, searchTerm, pageIndex, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(0); // PageIndex is 0-based in Pagination class
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(5);
        result.Items.Should().HaveCount(5);

        _mockLessonRepository.Verify(x => x.GetPaginatedLessonsByCourseAsync(
            courseId,
            searchTerm,
            1, // pageIndex normalized 
            pageSize), Times.Once);
    }

    [Fact]
    public async Task GetPaginatedAsync_WithNonExistentCourseId_ShouldThrowNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var pageIndex = 1;
        var pageSize = 10;
        var searchTerm = "";

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync((JCertPreApplication.Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _lessonService.GetPaginatedAsync(courseId, searchTerm, pageIndex, pageSize));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
    }

    #endregion

    #region CreateLessonAsync Tests

    [Fact]
    public async Task CreateLessonAsync_WithValidData_ShouldCreateLessonWithCorrectOrder()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var createDto = LessonServiceFixture.ValidCreateDto();
        var course = LessonServiceFixture.CreateTestCourse();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockLessonRepository.Setup(x => x.CountAsync(It.IsAny<Expression<Func<Lesson, bool>>>()))
            .ReturnsAsync(0);
        _mockLessonRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Lesson, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(new List<Lesson>());
        _mockLessonRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockLessonRepository.Setup(x => x.InsertAsync(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson lesson) => lesson);

        // Act
        var result = await _lessonService.CreateLessonAsync(courseId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.title.Should().Be(createDto.Title);
        result.content.Should().Be(createDto.Content);
        result.lessonOrder.Should().Be(1); // Should be set to 1 when no lessons exist

        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockLessonRepository.Verify(x => x.InsertAsync(It.Is<Lesson>(l => 
            l.courseId == courseId && 
            l.title == createDto.Title && 
            l.content == createDto.Content &&
            l.lessonOrder == 1)), Times.Once);
    }

    [Fact]
    public async Task CreateLessonAsync_WithNonExistentCourseId_ShouldThrowNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var createDto = LessonServiceFixture.ValidCreateDto();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync((JCertPreApplication.Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _lessonService.CreateLessonAsync(courseId, createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockLessonRepository.Verify(x => x.InsertAsync(It.IsAny<Lesson>()), Times.Never);
    }

    #endregion

    #region UpdateLessonAsync Tests

    [Fact]
    public async Task UpdateLessonAsync_WithValidData_ShouldUpdateLessonProperties()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var updateDto = LessonServiceFixture.ValidUpdateDto();
        var existingLesson = LessonBuilder.Create()
            .WithId(lessonId)
            .WithTitle("Old Title")
            .WithContent("Old Content")
            .WithOrder(1)
            .Build();

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(existingLesson);
        _mockLessonRepository.Setup(x => x.CountAsync(It.IsAny<Expression<Func<Lesson, bool>>>()))
            .ReturnsAsync(3);
        _mockLessonRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Lesson, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(new List<Lesson>());
        _mockLessonRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockLessonRepository.Setup(x => x.UpdateAsync(It.IsAny<Lesson>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _lessonService.UpdateLessonAsync(lessonId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.title.Should().Be(updateDto.Title);
        result.content.Should().Be(updateDto.Content);
        result.lessonOrder.Should().Be(updateDto.LessonOrder);

        _mockLessonRepository.Verify(x => x.UpdateAsync(It.Is<Lesson>(l => 
            l.lessonId == lessonId && 
            l.title == updateDto.Title && 
            l.content == updateDto.Content &&
            l.lessonOrder == updateDto.LessonOrder)), Times.Once);
    }

    [Fact]
    public async Task UpdateLessonAsync_WithNonExistentLessonId_ShouldThrowNotFoundException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var updateDto = LessonServiceFixture.ValidUpdateDto();

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _lessonService.UpdateLessonAsync(lessonId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockLessonRepository.Verify(x => x.UpdateAsync(It.IsAny<Lesson>()), Times.Never);
    }

    #endregion

    #region DeleteLessonByIdAsync Tests

    [Fact]
    public async Task DeleteLessonByIdAsync_WithValidId_ShouldDeleteAndReorderLessons()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var existingLesson = LessonBuilder.Create()
            .WithId(lessonId)
            .WithCourseId(courseId)
            .WithOrder(2)
            .Build();
        var lessonsToReorder = LessonServiceFixture.CreateLessonsForReorder(courseId, 2);

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(existingLesson);
        _mockLessonRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Lesson, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(lessonsToReorder);
        _mockLessonRepository.Setup(x => x.DeleteAsync(It.IsAny<Lesson>()))
            .Returns(Task.CompletedTask);
        _mockLessonRepository.Setup(x => x.UpdateAsync(It.IsAny<Lesson>()))
            .Returns(Task.CompletedTask);
        _mockLessonRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _lessonService.DeleteLessonByIdAsync(lessonId);

        // Assert (no return value to check)

        _mockLessonRepository.Verify(x => x.DeleteAsync(It.IsAny<Lesson>()), Times.Once);
        _mockLessonRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<Lesson, bool>>>(), It.IsAny<string?>()), Times.Once);
        _mockLessonRepository.Verify(x => x.UpdateAsync(It.IsAny<Lesson>()), Times.Exactly(2)); // Reorder remaining lessons
    }

    [Fact]
    public async Task DeleteLessonByIdAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();

        _mockLessonRepository.Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _lessonService.DeleteLessonByIdAsync(lessonId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockLessonRepository.Verify(x => x.DeleteAsync(It.IsAny<Lesson>()), Times.Never);
    }

    #endregion

    #region DeleteAllByCourseIdAsync Tests

    [Fact]
    public async Task DeleteAllByCourseIdAsync_WithValidCourseId_ShouldDeleteAllLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = LessonServiceFixture.CreateTestCourse();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mockLessonRepository.Setup(x => x.DeleteAllByCourseIdAsync(courseId))
            .Returns(Task.CompletedTask);
        _mockLessonRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _lessonService.DeleteAllByCourseIdAsync(courseId);

        // Assert (no return value to check)

        _mockCourseRepository.Verify(x => x.GetByIdAsync(courseId), Times.Once);
        _mockLessonRepository.Verify(x => x.DeleteAllByCourseIdAsync(courseId), Times.Once);
    }

    [Fact]
    public async Task DeleteAllByCourseIdAsync_WithNonExistentCourseId_ShouldThrowNotFoundException()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        _mockCourseRepository.Setup(x => x.GetByIdAsync(courseId))
            .ReturnsAsync((JCertPreApplication.Domain.Entities.Course?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _lessonService.DeleteAllByCourseIdAsync(courseId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

        _mockLessonRepository.Verify(x => x.DeleteAllByCourseIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    #endregion
}