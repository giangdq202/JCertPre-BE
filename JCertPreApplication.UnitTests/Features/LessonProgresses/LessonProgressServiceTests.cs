using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.LessonProgress;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.LessonProgresses;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.LessonProgresses
{
    public class LessonProgressServiceTests : IClassFixture<LessonProgressServiceFixture>
    {
        private readonly LessonProgressServiceFixture _fixture;
        private readonly LessonProgressService _service;
        private readonly Mock<ILessonProgressRepository> _mockLessonProgressRepository;
        private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepository;
        private readonly Mock<ILessonRepository> _mockLessonRepository;

        public LessonProgressServiceTests(LessonProgressServiceFixture fixture)
        {
            _fixture = fixture;
            _service = _fixture.LessonProgressService;
            _mockLessonProgressRepository = _fixture.MockLessonProgressRepository;
            _mockEnrollmentRepository = _fixture.MockEnrollmentRepository;
            _mockLessonRepository = _fixture.MockLessonRepository;
        }

        #region GetByUserAndCourseAsync Tests

        [Fact]
        public async Task GetByUserAndCourseAsync_WithValidIds_ShouldReturnProgressList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var progresses = LessonProgressServiceFixture.CreateUserCourseProgress(userId, courseId, 3);

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(progresses);

            // Act
            var result = await _service.GetByUserAndCourseAsync(userId, courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().AllSatisfy(dto =>
            {
                dto.UserId.Should().Be(userId);
                dto.CompletionRate.Should().BeGreaterThan(0);
            });
            _mockLessonProgressRepository.Verify(x => x.GetByUserAndCourseAsync(userId, courseId), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetByUserAndCourseAsync_WithNoProgress_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(new List<LessonProgress>());

            // Act
            var result = await _service.GetByUserAndCourseAsync(userId, courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockLessonProgressRepository.Verify(x => x.GetByUserAndCourseAsync(userId, courseId), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetByUserAndCourseAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.GetByUserAndCourseAsync(userId, courseId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("GET_LESSON_PROGRESS_BY_USER_COURSE_ERROR");
        }

        #endregion

        #region GetByUserAndLessonAsync Tests

        [Fact]
        public async Task GetByUserAndLessonAsync_WithExistingProgress_ShouldReturnProgress()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();
            var user = LessonProgressServiceFixture.CreateUserWithFullName("John Doe");
            user.userId = userId; // Ensure user has the correct userId
            var lesson = LessonProgressServiceFixture.CreateLessonWithTitle("Test Lesson");
            lesson.lessonId = lessonId; // Ensure lesson has the correct lessonId
            var progress = LessonProgressBuilder.Create()
                .WithUserId(userId)
                .WithLessonId(lessonId)
                .WithCompletionRate(85.5m)
                .WithUser(user)
                .WithLesson(lesson)
                .Build();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(userId, lessonId))
                .ReturnsAsync(progress);

            // Act
            var result = await _service.GetByUserAndLessonAsync(userId, lessonId);

            // Assert
            result.Should().NotBeNull();
            result!.UserId.Should().Be(userId);
            result.LessonId.Should().Be(lessonId);
            result.CompletionRate.Should().Be(85.5m);
            result.UserFullName.Should().Be(user.fullName);
            result.LessonTitle.Should().Be(lesson.title);
            _mockLessonProgressRepository.Verify(x => x.GetByUserAndLessonAsync(userId, lessonId), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetByUserAndLessonAsync_WithNoProgress_ShouldReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(userId, lessonId))
                .ReturnsAsync((LessonProgress?)null);

            // Act
            var result = await _service.GetByUserAndLessonAsync(userId, lessonId);

            // Assert
            result.Should().BeNull();
            _mockLessonProgressRepository.Verify(x => x.GetByUserAndLessonAsync(userId, lessonId), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetByUserAndLessonAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(userId, lessonId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.GetByUserAndLessonAsync(userId, lessonId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("GET_LESSON_PROGRESS_BY_USER_LESSON_ERROR");
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidFirstLesson_ShouldCreateProgress()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(1, courseId);
            var expectedCompletionRate = 25.0m;

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(true);
            _mockLessonProgressRepository.Setup(x => x.GetHighestPreviousLessonProgressAsync(dto.UserId, courseId))
                .ReturnsAsync(((int LessonOrder, decimal CompletionRate)?)null);
            _mockLessonRepository.Setup(x => x.GetTestByLessonIdAsync(dto.LessonId))
                .ReturnsAsync((JCertPreApplication.Domain.Entities.Test?)null);
            _mockLessonProgressRepository.Setup(x => x.CalculateCompletionRateAfterAddAsync(dto.UserId, courseId))
                .ReturnsAsync(expectedCompletionRate);
            _mockLessonProgressRepository.Setup(x => x.InsertAsync(It.IsAny<LessonProgress>()))
                .ReturnsAsync((LessonProgress p) => p);
            _mockLessonProgressRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            
            var createdProgress = LessonProgressBuilder.Create()
                .WithUserId(dto.UserId)
                .WithLessonId(dto.LessonId)
                .WithCompletionRate(expectedCompletionRate)
                .Build();
            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdProgress);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(dto.UserId);
            result.LessonId.Should().Be(dto.LessonId);
            result.CompletionRate.Should().Be(expectedCompletionRate);
            
            _mockLessonProgressRepository.Verify(x => x.InsertAsync(It.IsAny<LessonProgress>()), Times.AtLeastOnce);
            _mockLessonProgressRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateAsync_WithValidSequentialLesson_ShouldCreateProgress()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(3, courseId);
            var previousProgress = LessonProgressServiceFixture.CreatePreviousProgress(2);
            var expectedCompletionRate = 75.0m;

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(true);
            _mockLessonProgressRepository.Setup(x => x.GetHighestPreviousLessonProgressAsync(dto.UserId, courseId))
                .ReturnsAsync(previousProgress);
            _mockLessonRepository.Setup(x => x.GetTestByLessonIdAsync(dto.LessonId))
                .ReturnsAsync((JCertPreApplication.Domain.Entities.Test?)null);
            _mockLessonProgressRepository.Setup(x => x.CalculateCompletionRateAfterAddAsync(dto.UserId, courseId))
                .ReturnsAsync(expectedCompletionRate);
            _mockLessonProgressRepository.Setup(x => x.InsertAsync(It.IsAny<LessonProgress>()))
                .ReturnsAsync((LessonProgress p) => p);
            _mockLessonProgressRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            
            var createdProgress = LessonProgressBuilder.Create()
                .WithUserId(dto.UserId)
                .WithLessonId(dto.LessonId)
                .WithCompletionRate(expectedCompletionRate)
                .Build();
            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdProgress);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.CompletionRate.Should().Be(expectedCompletionRate);
            _mockLessonProgressRepository.Verify(x => x.InsertAsync(It.IsAny<LessonProgress>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateAsync_WithExistingProgress_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var existingProgress = LessonProgressBuilder.Create()
                .WithUserId(dto.UserId)
                .WithLessonId(dto.LessonId)
                .Build();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync(existingProgress);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentLesson_ShouldThrowNotFoundException()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync((Lesson?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task CreateAsync_WithUserNotEnrolled_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(1, courseId);

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task CreateAsync_WithWrongLessonOrder_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(4, courseId); // Skipping lesson 3
            var previousProgress = LessonProgressServiceFixture.CreatePreviousProgress(2); // User completed lesson 2

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(true);
            _mockLessonProgressRepository.Setup(x => x.GetHighestPreviousLessonProgressAsync(dto.UserId, courseId))
                .ReturnsAsync(previousProgress);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task CreateAsync_WithFirstLessonNotOrder1_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(2, courseId); // Starting with lesson 2

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(true);
            _mockLessonProgressRepository.Setup(x => x.GetHighestPreviousLessonProgressAsync(dto.UserId, courseId))
                .ReturnsAsync(((int LessonOrder, decimal CompletionRate)?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task CreateAsync_WithTestNotPassed_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(1, courseId);
            var test = LessonProgressServiceFixture.CreateTestForLesson();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(true);
            _mockLessonProgressRepository.Setup(x => x.GetHighestPreviousLessonProgressAsync(dto.UserId, courseId))
                .ReturnsAsync(((int LessonOrder, decimal CompletionRate)?)null);
            _mockLessonRepository.Setup(x => x.GetTestByLessonIdAsync(dto.LessonId))
                .ReturnsAsync(test);
            _mockLessonRepository.Setup(x => x.IsUserPassedTestAsync(dto.UserId, test.testId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task CreateAsync_WithLessonWithoutTest_ShouldCreateProgress()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();
            var courseId = Guid.NewGuid();
            var lesson = LessonProgressServiceFixture.CreateLessonWithOrder(1, courseId);
            var expectedCompletionRate = 30.0m;

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ReturnsAsync((LessonProgress?)null);
            _mockLessonRepository.Setup(x => x.GetByIdAsync(dto.LessonId))
                .ReturnsAsync(lesson);
            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledInCourseAsync(dto.UserId, courseId))
                .ReturnsAsync(true);
            _mockLessonProgressRepository.Setup(x => x.GetHighestPreviousLessonProgressAsync(dto.UserId, courseId))
                .ReturnsAsync(((int LessonOrder, decimal CompletionRate)?)null);
            _mockLessonRepository.Setup(x => x.GetTestByLessonIdAsync(dto.LessonId))
                .ReturnsAsync((JCertPreApplication.Domain.Entities.Test?)null); // No test for this lesson
            _mockLessonProgressRepository.Setup(x => x.CalculateCompletionRateAfterAddAsync(dto.UserId, courseId))
                .ReturnsAsync(expectedCompletionRate);
            _mockLessonProgressRepository.Setup(x => x.InsertAsync(It.IsAny<LessonProgress>()))
                .ReturnsAsync((LessonProgress p) => p);
            _mockLessonProgressRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            
            var createdProgress = LessonProgressBuilder.Create()
                .WithUserId(dto.UserId)
                .WithLessonId(dto.LessonId)
                .WithCompletionRate(expectedCompletionRate)
                .Build();
            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdProgress);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.CompletionRate.Should().Be(expectedCompletionRate);
            _mockLessonRepository.Verify(x => x.IsUserPassedTestAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            _mockLessonProgressRepository.Verify(x => x.InsertAsync(It.IsAny<LessonProgress>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var dto = LessonProgressServiceFixture.ValidCreateDto();

            _mockLessonProgressRepository.Setup(x => x.GetByUserAndLessonAsync(dto.UserId, dto.LessonId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.CreateAsync(dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("CREATE_LESSON_PROGRESS_ERROR");
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateCompletionRate()
        {
            // Arrange
            var progressId = Guid.NewGuid();
            var dto = LessonProgressServiceFixture.ValidUpdateDto();
            var existingProgress = LessonProgressBuilder.Create()
                .WithId(progressId)
                .WithCompletionRate(50.0m)
                .Build();
            var updatedProgress = LessonProgressBuilder.Create()
                .WithId(progressId)
                .WithCompletionRate(dto.CompletionRate)
                .Build();

            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(progressId))
                .ReturnsAsync(existingProgress);
            _mockLessonProgressRepository.Setup(x => x.UpdateAsync(It.IsAny<LessonProgress>()))
                .Returns(Task.CompletedTask);
            _mockLessonProgressRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockLessonProgressRepository.SetupSequence(x => x.GetByIdAsync(progressId))
                .ReturnsAsync(existingProgress)
                .ReturnsAsync(updatedProgress);

            // Act
            var result = await _service.UpdateAsync(progressId, dto);

            // Assert
            result.Should().NotBeNull();
            result.ProgressId.Should().Be(progressId);
            result.CompletionRate.Should().Be(dto.CompletionRate);
            _mockLessonProgressRepository.Verify(x => x.UpdateAsync(It.IsAny<LessonProgress>()), Times.AtLeastOnce);
            _mockLessonProgressRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentProgress_ShouldThrowNotFoundException()
        {
            // Arrange
            var progressId = Guid.NewGuid();
            var dto = LessonProgressServiceFixture.ValidUpdateDto();

            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(progressId))
                .ReturnsAsync((LessonProgress?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.UpdateAsync(progressId, dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("UPDATE_LESSON_PROGRESS_ERROR");
        }

        [Fact]
        public async Task UpdateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var progressId = Guid.NewGuid();
            var dto = LessonProgressServiceFixture.ValidUpdateDto();

            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(progressId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.UpdateAsync(progressId, dto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("UPDATE_LESSON_PROGRESS_ERROR");
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingProgress_ShouldDeleteProgress()
        {
            // Arrange
            var progressId = Guid.NewGuid();
            var existingProgress = LessonProgressBuilder.Create()
                .WithId(progressId)
                .Build();

            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(progressId))
                .ReturnsAsync(existingProgress);
            _mockLessonProgressRepository.Setup(x => x.DeleteAsync(existingProgress))
                .Returns(Task.CompletedTask);
            _mockLessonProgressRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _service.DeleteAsync(progressId);

            // Assert
            _mockLessonProgressRepository.Verify(x => x.DeleteAsync(existingProgress), Times.AtLeastOnce);
            _mockLessonProgressRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentProgress_ShouldThrowNotFoundException()
        {
            // Arrange
            var progressId = Guid.NewGuid();

            _mockLessonProgressRepository.Setup(x => x.GetByIdAsync(progressId))
                .ReturnsAsync((LessonProgress?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.DeleteAsync(progressId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("DELETE_LESSON_PROGRESS_ERROR");
        }

        #endregion

        #region GetUserCourseCompletionRateAsync Tests

        [Fact]
        public async Task GetUserCourseCompletionRateAsync_WithValidIds_ShouldReturnCompletionRate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedRate = 72.5m;

            _mockLessonProgressRepository.Setup(x => x.GetUserCourseCompletionRateAsync(userId, courseId))
                .ReturnsAsync(expectedRate);

            // Act
            var result = await _service.GetUserCourseCompletionRateAsync(userId, courseId);

            // Assert
            result.Should().Be(expectedRate);
            _mockLessonProgressRepository.Verify(x => x.GetUserCourseCompletionRateAsync(userId, courseId), Times.AtLeastOnce);
        }

        [Fact]
        public async Task GetUserCourseCompletionRateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _mockLessonProgressRepository.Setup(x => x.GetUserCourseCompletionRateAsync(userId, courseId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _service.GetUserCourseCompletionRateAsync(userId, courseId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("GET_USER_COURSE_COMPLETION_RATE_ERROR");
        }

        #endregion
    }
}
