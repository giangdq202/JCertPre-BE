using FluentAssertions;
using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;

namespace JCertPreApplication.UnitTests.Features.Test;

public class TestServiceTests : IClassFixture<TestServiceFixture>
{
    private readonly TestServiceFixture _fixture;

    public TestServiceTests(TestServiceFixture fixture)
    {
        _fixture = fixture;
    }

    #region GetByTestIdAsync Tests

    [Fact]
    public async Task GetByTestIdAsync_WithExistingId_ShouldReturnTest()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithTitle("Sample Test")
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        // Act
        var result = await _fixture.TestService.GetByTestIdAsync(testId);

        // Assert
        result.Should().NotBeNull();
        result!.TestId.Should().Be(testId);
        result.Title.Should().Be("Sample Test");

        _fixture.MockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.Once);
    }

    [Fact]
    public async Task GetByTestIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var testId = Guid.NewGuid();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync((Domain.Entities.Test?)null);

        // Act
        var result = await _fixture.TestService.GetByTestIdAsync(testId);

        // Assert
        result.Should().BeNull();

        _fixture.MockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.Once);
    }

    #endregion

    #region GetAllByUserIdAsync Tests

    [Fact]
    public async Task GetAllByUserIdAsync_WithValidParams_ShouldReturnPaginatedTests()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var searchTerm = "test";
        var pageIndex = 1;
        var pageSize = 10;

        var tests = new List<Domain.Entities.Test>
        {
            TestBuilder.Create().WithTitle("Test 1").WithCreatedByUserId(userId).Build(),
            TestBuilder.Create().WithTitle("Test 2").WithCreatedByUserId(userId).Build()
        };

        var paginatedResult = new Pagination<Domain.Entities.Test>
        {
            Items = tests,
            TotalItemsCount = 2,
            PageIndex = 1,
            PageSize = 10
        };

        _fixture.MockTestRepository
            .Setup(x => x.GetPaginationAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                "TestTemplateType",
                1,
                10,
                It.IsAny<Func<IQueryable<Domain.Entities.Test>, IOrderedQueryable<Domain.Entities.Test>>>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _fixture.TestService.GetAllByUserIdAsync(userId, searchTerm, pageIndex, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalItemsCount.Should().Be(2);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);

        _fixture.MockTestRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
            "TestTemplateType",
            1,
            10,
            It.IsAny<Func<IQueryable<Domain.Entities.Test>, IOrderedQueryable<Domain.Entities.Test>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_WithEmptyResults_ShouldReturnEmptyPagination()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyResult = new Pagination<Domain.Entities.Test>
        {
            Items = new List<Domain.Entities.Test>(),
            TotalItemsCount = 0,
            PageIndex = 1,
            PageSize = 10
        };

        _fixture.MockTestRepository
            .Setup(x => x.GetPaginationAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                "TestTemplateType",
                1,
                10,
                It.IsAny<Func<IQueryable<Domain.Entities.Test>, IOrderedQueryable<Domain.Entities.Test>>>()))
            .ReturnsAsync(emptyResult);

        // Act
        var result = await _fixture.TestService.GetAllByUserIdAsync(userId, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalItemsCount.Should().Be(0);
    }

    #endregion

    #region GetByLessonIdAsync Tests

    [Fact]
    public async Task GetByLessonIdAsync_WithExistingLessonId_ShouldReturnTest()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var existingTest = TestBuilder.Create()
            .WithLessonId(lessonId)
            .WithTitle("Lesson Test")
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                "TestTemplateType"))
            .ReturnsAsync(existingTest);

        // Act
        var result = await _fixture.TestService.GetByLessonIdAsync(lessonId);

        // Assert
        result.Should().NotBeNull();
        result!.LessonId.Should().Be(lessonId);
        result.Title.Should().Be("Lesson Test");

        _fixture.MockTestRepository.Verify(x => x.GetFirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
            "TestTemplateType"), Times.Once);
    }

    [Fact]
    public async Task GetByLessonIdAsync_WithNonExistentLessonId_ShouldThrowNotFoundException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();

        _fixture.MockTestRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                "TestTemplateType"))
            .ReturnsAsync((Domain.Entities.Test?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.GetByLessonIdAsync(lessonId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
    }

    #endregion

    #region CreateByLessonIdAsync Tests

    [Fact]
    public async Task CreateByLessonIdAsync_WithValidData_ShouldCreateTest()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createDto = new CreateTestDto
        {
            Title = "New Test",
            Description = "Test Description",
            TestType = TestType.CustomManual,
            CourseLevel = CourseLevel.N5,
            DurationMinutes = 60,
            AvailableFrom = DateTime.UtcNow,
            AvailableTo = DateTime.UtcNow.AddDays(7),
            MaxAttempts = 3,
            PassingPercentage = 70
        };

        var lesson = LessonBuilder.Create().WithId(lessonId).Build();

        _fixture.MockLessonRepository
            .Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _fixture.MockTestRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Test?)null);

        _fixture.MockTestRepository
            .Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Test>()))
            .ReturnsAsync((Domain.Entities.Test test) => test);

        _fixture.MockTestRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var createdTest = TestBuilder.Create()
            .WithTitle(createDto.Title)
            .WithDescription(createDto.Description)
            .WithLessonId(lessonId)
            .WithCreatedByUserId(userId)
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(createdTest);

        // Act
        var result = await _fixture.TestService.CreateByLessonIdAsync(lessonId, createDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createDto.Title);
        result.Description.Should().Be(createDto.Description);
        result.LessonId.Should().Be(lessonId);
        result.CreatedByUserId.Should().Be(userId);

        _fixture.MockLessonRepository.Verify(x => x.GetByIdAsync(lessonId), Times.AtLeastOnce);
        _fixture.MockTestRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Test>()), Times.AtLeastOnce);
        _fixture.MockTestRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CreateByLessonIdAsync_WithNonExistentLessonId_ShouldThrowNotFoundException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createDto = new CreateTestDto
        {
            Title = "New Test",
            TestType = TestType.CustomManual,
            CourseLevel = CourseLevel.N5,
            DurationMinutes = 60,
            MaxAttempts = 3,
            PassingPercentage = 70
        };

        _fixture.MockLessonRepository
            .Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateByLessonIdAsync(lessonId, createDto, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
    }

    [Fact]
    public async Task CreateByLessonIdAsync_WithExistingTest_ShouldThrowBadRequestException()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createDto = new CreateTestDto
        {
            Title = "New Test",
            TestType = TestType.CustomManual,
            CourseLevel = CourseLevel.N5,
            DurationMinutes = 60,
            MaxAttempts = 3,
            PassingPercentage = 70
        };

        var lesson = LessonBuilder.Create().WithId(lessonId).Build();
        var existingTest = TestBuilder.Create().WithLessonId(lessonId).Build();

        _fixture.MockLessonRepository
            .Setup(x => x.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);

        _fixture.MockTestRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(existingTest);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateByLessonIdAsync(lessonId, createDto, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("TEST_ALREADY_EXISTS");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateTest()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var updateDto = new UpdateTestDto
        {
            Title = "Updated Test Title",
            Description = "Updated Description",
            DurationMinutes = 90
        };

        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithTitle("Original Title")
            .WithStatus(TestStatus.Close)
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        _fixture.MockTestRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Test>()))
            .Returns(Task.CompletedTask);

        _fixture.MockTestRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var updatedTest = TestBuilder.Create()
            .WithId(testId)
            .WithTitle(updateDto.Title)
            .WithDescription(updateDto.Description!)
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Test, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(updatedTest);

        // Act
        var result = await _fixture.TestService.UpdateAsync(testId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(updateDto.Title);
        result.Description.Should().Be(updateDto.Description);

        _fixture.MockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.AtLeastOnce);
        _fixture.MockTestRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Test>()), Times.AtLeastOnce);
        _fixture.MockTestRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var updateDto = new UpdateTestDto
        {
            Title = "Updated Test Title"
        };

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync((Domain.Entities.Test?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.UpdateAsync(testId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
    }

    [Fact]
    public async Task UpdateAsync_WithPassingPercentageOnOpenTest_ShouldThrowBadRequestException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var updateDto = new UpdateTestDto
        {
            PassingPercentage = 80
        };

        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithStatus(TestStatus.Open) // Open status should not allow PassingPercentage update
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.UpdateAsync(testId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("UPDATE_PASSING_PERCENTAGE_NOT_ALLOWED");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidClosedTest_ShouldDeleteTest()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithStatus(TestStatus.Close) // Must be closed to delete
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        _fixture.MockTestAttemptRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestAttempt, bool>>>()))
            .ReturnsAsync(false); // No active attempts

        _fixture.MockTestQuestionRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestQuestion, bool>>>()))
            .ReturnsAsync(false); // No test questions

        _fixture.MockTestRepository
            .Setup(x => x.DeleteAsync(It.IsAny<Domain.Entities.Test>()))
            .Returns(Task.CompletedTask);

        _fixture.MockTestRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _fixture.TestService.DeleteAsync(testId);

        // Assert
        _fixture.MockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.AtLeastOnce);
        _fixture.MockTestRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Test>()), Times.AtLeastOnce);
        _fixture.MockTestRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var testId = Guid.NewGuid();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync((Domain.Entities.Test?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.DeleteAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
    }

    [Fact]
    public async Task DeleteAsync_WithOpenTest_ShouldThrowBadRequestException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithStatus(TestStatus.Open) // Open status should not allow deletion
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.DeleteAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
    }

    [Fact]
    public async Task DeleteAsync_WithActiveAttempt_ShouldThrowBadRequestException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithStatus(TestStatus.Close)
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        _fixture.MockTestAttemptRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestAttempt, bool>>>()))
            .ReturnsAsync(true); // Has active attempts

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.DeleteAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
    }

    [Fact]
    public async Task DeleteAsync_WithTestQuestions_ShouldThrowBadRequestException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var existingTest = TestBuilder.Create()
            .WithId(testId)
            .WithStatus(TestStatus.Close)
            .Build();

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(existingTest);

        _fixture.MockTestAttemptRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestAttempt, bool>>>()))
            .ReturnsAsync(false); // No active attempts

        _fixture.MockTestQuestionRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestQuestion, bool>>>()))
            .ReturnsAsync(true); // Has test questions

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.DeleteAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
    }

    #endregion

    #region CreateAutoTestAndAddQuestionsAsync Tests

    [Fact]
    public async Task CreateAutoTestAsync_WithValidTemplate_ShouldCreateTest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateAutoTestInput
        {
            TestType = TestType.JLPTAuto,
            CourseLevel = CourseLevel.N5
        };

        var studentProfile = StudentProfileBuilder.Create()
            .WithUserId(userId)
            .WithNumberOfTestsTaken(5) // Below limit
            .WithLastResetTestTime(DateTime.UtcNow.Date) // Reset today
            .Build();

        var templateType = TestTemplateTypeBuilder.Create()
            .WithCourseLevel(CourseLevel.N5)
            .WithTestType(TestType.JLPTAuto)
            .AsActive()
            .Build();

        var templates = new List<TestTemplate>
        {
            TestTemplateBuilder.Create()
                .WithTestTemplateTypeId(templateType.TestTemplateTypeId)
                .WithDurationMinutes(30)
                .Build(),
            TestTemplateBuilder.Create()
                .WithTestTemplateTypeId(templateType.TestTemplateTypeId)
                .WithDurationMinutes(30)
                .Build()
        };

        _fixture.MockEnrollmentRepository
            .Setup(x => x.IsUserEnrolledInAnyCourseAsync(userId))
            .ReturnsAsync(true);

        _fixture.MockStudentProfileRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StudentProfile, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(studentProfile);

        _fixture.MockTestTemplateTypeRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(templateType);

        _fixture.MockTestTemplateRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestTemplate, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(templates);

        _fixture.MockTestRepository
            .Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Test>()))
            .ReturnsAsync((Domain.Entities.Test test) => test);

        _fixture.MockTestRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _fixture.MockTestRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Test>()))
            .Returns(Task.CompletedTask);

        _fixture.MockStudentProfileRepository
            .Setup(x => x.UpdateAsync(It.IsAny<StudentProfile>()))
            .Returns(Task.CompletedTask);

        _fixture.MockTestQuestionService
            .Setup(x => x.AddQuestionsJLPTAutoAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _fixture.TestService.CreateAutoTestAndAddQuestionsAsync(input, userId);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Contain("N5 Auto Test");
        result.DurationMinutes.Should().Be(60); // Sum of template durations
        result.Status.Should().Be(TestStatus.Open);

        _fixture.MockEnrollmentRepository.Verify(x => x.IsUserEnrolledInAnyCourseAsync(userId), Times.Once);
        _fixture.MockTestRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Test>()), Times.Once);
        _fixture.MockTestQuestionService.Verify(x => x.AddQuestionsJLPTAutoAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task CreateAutoTestAsync_WithUserNotEnrolled_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateAutoTestInput
        {
            TestType = TestType.JLPTAuto,
            CourseLevel = CourseLevel.N5
        };

        _fixture.MockEnrollmentRepository
            .Setup(x => x.IsUserEnrolledInAnyCourseAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateAutoTestAndAddQuestionsAsync(input, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("USER_NOT_ENROLLED");
    }

    [Fact]
    public async Task CreateAutoTestAsync_WithNoStudentProfile_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateAutoTestInput
        {
            TestType = TestType.JLPTAuto,
            CourseLevel = CourseLevel.N5
        };

        _fixture.MockEnrollmentRepository
            .Setup(x => x.IsUserEnrolledInAnyCourseAsync(userId))
            .ReturnsAsync(true);

        _fixture.MockStudentProfileRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StudentProfile, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((StudentProfile?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateAutoTestAndAddQuestionsAsync(input, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("NO_STUDENT_PROFILE");
    }

    [Fact]
    public async Task CreateAutoTestAsync_WithDailyLimitReached_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateAutoTestInput
        {
            TestType = TestType.JLPTAuto,
            CourseLevel = CourseLevel.N5
        };

        var studentProfile = StudentProfileBuilder.Create()
            .WithUserId(userId)
            .WithNumberOfTestsTaken(10) // At limit
            .WithLastResetTestTime(DateTime.UtcNow.Date) // Reset today
            .Build();

        _fixture.MockEnrollmentRepository
            .Setup(x => x.IsUserEnrolledInAnyCourseAsync(userId))
            .ReturnsAsync(true);

        _fixture.MockStudentProfileRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StudentProfile, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(studentProfile);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateAutoTestAndAddQuestionsAsync(input, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("TEST_LIMIT_REACHED");
    }

    [Fact]
    public async Task CreateAutoTestAsync_WithInvalidTemplate_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateAutoTestInput
        {
            TestType = TestType.JLPTAuto,
            CourseLevel = CourseLevel.N5
        };

        var studentProfile = StudentProfileBuilder.Create()
            .WithUserId(userId)
            .WithNumberOfTestsTaken(0)
            .Build();

        _fixture.MockEnrollmentRepository
            .Setup(x => x.IsUserEnrolledInAnyCourseAsync(userId))
            .ReturnsAsync(true);

        _fixture.MockStudentProfileRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StudentProfile, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(studentProfile);

        _fixture.MockTestTemplateTypeRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((TestTemplateType?)null); // No template found

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateAutoTestAndAddQuestionsAsync(input, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("NO_TEMPLATE_TYPE_FOUND");
    }

    [Fact]
    public async Task CreateAutoTestAsync_WithNoTemplates_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var input = new CreateAutoTestInput
        {
            TestType = TestType.JLPTAuto,
            CourseLevel = CourseLevel.N5
        };

        var studentProfile = StudentProfileBuilder.Create()
            .WithUserId(userId)
            .WithNumberOfTestsTaken(0)
            .Build();

        var templateType = TestTemplateTypeBuilder.Create()
            .WithCourseLevel(CourseLevel.N5)
            .WithTestType(TestType.JLPTAuto)
            .AsActive()
            .Build();

        _fixture.MockEnrollmentRepository
            .Setup(x => x.IsUserEnrolledInAnyCourseAsync(userId))
            .ReturnsAsync(true);

        _fixture.MockStudentProfileRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StudentProfile, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(studentProfile);

        _fixture.MockTestTemplateTypeRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(templateType);

        _fixture.MockTestTemplateRepository
            .Setup(x => x.GetAllAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<TestTemplate, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(new List<TestTemplate>()); // Empty templates

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _fixture.TestService.CreateAutoTestAndAddQuestionsAsync(input, userId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("NO_TEMPLATES_FOUND");
    }

    #endregion
}
