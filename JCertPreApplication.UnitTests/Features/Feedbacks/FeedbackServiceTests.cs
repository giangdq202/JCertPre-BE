using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Feedback;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Feedbacks;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.Feedbacks
{
    /// <summary>
    /// Unit tests for FeedbackService
    /// Testing feedback management including CRUD operations, business rules, and error handling
    /// </summary>
    public class FeedbackServiceTests
    {
        private readonly FeedbackServiceFixture _fixture;
        private readonly FeedbackService _feedbackService;
        private readonly Mock<IFeedbackRepository> _mockFeedbackRepository;
        private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepository;

        public FeedbackServiceTests()
        {
            _fixture = new FeedbackServiceFixture();
            _feedbackService = _fixture.FeedbackService;
            _mockFeedbackRepository = _fixture.MockFeedbackRepository;
            _mockEnrollmentRepository = _fixture.MockEnrollmentRepository;
        }

        #region GetPagingByCourseIdAsync Tests

        [Fact]
        public async Task GetPagingByCourseIdAsync_WithValidCourseId_ShouldReturnPaginatedFeedbacks()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;
            var feedbacks = FeedbackServiceFixture.CreateFeedbackList(courseId, 5);
            var paginatedResult = FeedbackServiceFixture.CreatePaginatedFeedbacks(feedbacks, pageIndex, pageSize, 5);

            _mockFeedbackRepository.Setup(x => x.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User"))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _feedbackService.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(5);
            result.PageIndex.Should().Be(pageIndex);
            result.PageSize.Should().Be(pageSize);
            result.TotalItemsCount.Should().Be(5);
            
            // Verify DTO mapping
            var firstFeedback = result.Items.First();
            firstFeedback.FeedbackId.Should().Be(feedbacks.First().feedbackId);
            firstFeedback.CourseId.Should().Be(courseId);
            firstFeedback.UserFullName.Should().Be(feedbacks.First().User.fullName);
            firstFeedback.UserAvatarUrl.Should().Be(feedbacks.First().User.avatarUrl);

            _mockFeedbackRepository.Verify(x => x.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User"), Times.Once);
        }

        [Fact]
        public async Task GetPagingByCourseIdAsync_WithNoFeedbacks_ShouldReturnEmptyPagination()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;
            var emptyResult = FeedbackServiceFixture.CreatePaginatedFeedbacks(new List<Feedback>(), pageIndex, pageSize, 0);

            _mockFeedbackRepository.Setup(x => x.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User"))
                .ReturnsAsync(emptyResult);

            // Act
            var result = await _feedbackService.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.TotalItemsCount.Should().Be(0);
            result.PageIndex.Should().Be(pageIndex);
            result.PageSize.Should().Be(pageSize);

            _mockFeedbackRepository.Verify(x => x.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User"), Times.Once);
        }

        [Fact]
        public async Task GetPagingByCourseIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var pageIndex = 1;
            var pageSize = 10;

            _mockFeedbackRepository.Setup(x => x.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User"))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("FEEDBACK_SERVICE_ERROR");
            exception.Message.Should().Contain("Error getting feedbacks");

            _mockFeedbackRepository.Verify(x => x.GetPagingByCourseIdAsync(courseId, pageIndex, pageSize, "User"), Times.Once);
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidEnrolledUser_ShouldCreateFeedback()
        {
            // Arrange
            var createDto = FeedbackServiceFixture.ValidCreateDto();
            var createdFeedback = FeedbackServiceFixture.CreateFeedbackWithUser(createDto.CourseId, createDto.UserId);

            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync(true);
            _mockFeedbackRepository.SetupSequence(x => x.GetByUserAndCourseAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync((Feedback?)null)
                .ReturnsAsync(createdFeedback);
            _mockFeedbackRepository.Setup(x => x.InsertAsync(It.IsAny<Feedback>()))
                .ReturnsAsync((Feedback f) => f);
            _mockFeedbackRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _feedbackService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(createDto.CourseId);
            result.UserId.Should().Be(createDto.UserId);
            result.Rating.Should().Be(createdFeedback.rating);
            result.Comment.Should().Be(createdFeedback.comment);
            result.UserFullName.Should().Be(createdFeedback.User.fullName);
            result.UserAvatarUrl.Should().Be(createdFeedback.User.avatarUrl);

            _mockEnrollmentRepository.Verify(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.InsertAsync(It.IsAny<Feedback>()), Times.Once);
            _mockFeedbackRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNonEnrolledUser_ShouldThrowForbiddenException()
        {
            // Arrange
            var createDto = FeedbackServiceFixture.ValidCreateDto();

            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.CreateAsync(createDto));

            exception.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            exception.ErrorCode.Should().Be("USER_NOT_ENROLLED");
            exception.Message.Should().Be("User must be enrolled in the course to leave feedback.");

            _mockEnrollmentRepository.Verify(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.InsertAsync(It.IsAny<Feedback>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithExistingFeedback_ShouldThrowBadRequestException()
        {
            // Arrange
            var createDto = FeedbackServiceFixture.ValidCreateDto();
            var existingFeedback = FeedbackBuilder.Create()
                .WithCourseId(createDto.CourseId)
                .WithUserId(createDto.UserId)
                .Build();

            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync(true);
            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync(existingFeedback);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.CreateAsync(createDto));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("FEEDBACK_EXISTS");
            exception.Message.Should().Be("Feedback already exists for this user and course.");

            _mockEnrollmentRepository.Verify(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.GetByUserAndCourseAsync(createDto.UserId, createDto.CourseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.InsertAsync(It.IsAny<Feedback>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenEnrollmentCheckFails_ShouldThrowInternalServerError()
        {
            // Arrange
            var createDto = FeedbackServiceFixture.ValidCreateDto();

            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.CreateAsync(createDto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("FEEDBACK_SERVICE_ERROR");
            exception.Message.Should().Contain("Error creating feedback");

            _mockEnrollmentRepository.Verify(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var createDto = FeedbackServiceFixture.ValidCreateDto();

            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync(true);
            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(createDto.UserId, createDto.CourseId))
                .ReturnsAsync((Feedback?)null);
            _mockFeedbackRepository.Setup(x => x.InsertAsync(It.IsAny<Feedback>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.CreateAsync(createDto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("FEEDBACK_SERVICE_ERROR");
            exception.Message.Should().Contain("Error creating feedback");

            _mockFeedbackRepository.Verify(x => x.InsertAsync(It.IsAny<Feedback>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithApiExceptionFromRepository_ShouldRethrowApiException()
        {
            // Arrange
            var createDto = FeedbackServiceFixture.ValidCreateDto();
            var apiException = ApiException.BadRequest("CUSTOM_ERROR", "Custom error message");

            _mockEnrollmentRepository.Setup(x => x.IsUserEnrolledAsync(createDto.UserId, createDto.CourseId))
                .ThrowsAsync(apiException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.CreateAsync(createDto));

            exception.Should().Be(apiException);
            exception.ErrorCode.Should().Be("CUSTOM_ERROR");
            exception.Message.Should().Be("Custom error message");
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateFeedback()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var updateDto = FeedbackServiceFixture.ValidUpdateDto();
            var existingFeedback = FeedbackBuilder.Create()
                .WithUserId(userId)
                .WithCourseId(courseId)
                .WithRating(3.0m)
                .WithComment("Old comment")
                .Build();
            var updatedFeedback = FeedbackServiceFixture.CreateFeedbackWithUser(courseId, userId);
            updatedFeedback.rating = updateDto.Rating!.Value;
            updatedFeedback.comment = updateDto.Comment;

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback);
            _mockFeedbackRepository.Setup(x => x.UpdateAsync(existingFeedback))
                .Returns(Task.CompletedTask);
            _mockFeedbackRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockFeedbackRepository.SetupSequence(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback)
                .ReturnsAsync(updatedFeedback);

            // Act
            var result = await _feedbackService.UpdateAsync(userId, courseId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Rating.Should().Be(updateDto.Rating);
            result.Comment.Should().Be(updateDto.Comment);
            result.UserId.Should().Be(userId);
            result.CourseId.Should().Be(courseId);

            // Verify the existing feedback was updated
            existingFeedback.rating.Should().Be(updateDto.Rating!.Value);
            existingFeedback.comment.Should().Be(updateDto.Comment);

            _mockFeedbackRepository.Verify(x => x.GetByUserAndCourseAsync(userId, courseId), Times.Exactly(2));
            _mockFeedbackRepository.Verify(x => x.UpdateAsync(existingFeedback), Times.Once);
            _mockFeedbackRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithPartialData_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var updateDto = FeedbackServiceFixture.UpdateDtoWithRatingOnly(); // Only rating, comment is null
            var existingFeedback = FeedbackBuilder.Create()
                .WithUserId(userId)
                .WithCourseId(courseId)
                .WithRating(4.0m)
                .WithComment("Original comment")
                .Build();
            var updatedFeedback = FeedbackServiceFixture.CreateFeedbackWithUser(courseId, userId);
            updatedFeedback.rating = updateDto.Rating!.Value;
            updatedFeedback.comment = "Original comment"; // Should remain unchanged

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback);
            _mockFeedbackRepository.Setup(x => x.UpdateAsync(existingFeedback))
                .Returns(Task.CompletedTask);
            _mockFeedbackRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);
            _mockFeedbackRepository.SetupSequence(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback)
                .ReturnsAsync(updatedFeedback);

            // Act
            var result = await _feedbackService.UpdateAsync(userId, courseId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Rating.Should().Be(updateDto.Rating);

            // Verify only rating was updated, comment remains unchanged
            existingFeedback.rating.Should().Be(updateDto.Rating!.Value);
            existingFeedback.comment.Should().Be("Original comment"); // Should not change

            _mockFeedbackRepository.Verify(x => x.UpdateAsync(existingFeedback), Times.Once);
            _mockFeedbackRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentFeedback_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var updateDto = FeedbackServiceFixture.ValidUpdateDto();

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync((Feedback?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.UpdateAsync(userId, courseId, updateDto));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            exception.Message.Should().Contain($"userId={userId}, courseId={courseId}");

            _mockFeedbackRepository.Verify(x => x.GetByUserAndCourseAsync(userId, courseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.UpdateAsync(It.IsAny<Feedback>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var updateDto = FeedbackServiceFixture.ValidUpdateDto();
            var existingFeedback = FeedbackBuilder.Create()
                .WithUserId(userId)
                .WithCourseId(courseId)
                .Build();

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback);
            _mockFeedbackRepository.Setup(x => x.UpdateAsync(existingFeedback))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.UpdateAsync(userId, courseId, updateDto));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("FEEDBACK_SERVICE_ERROR");
            exception.Message.Should().Contain("Error updating feedback");

            _mockFeedbackRepository.Verify(x => x.UpdateAsync(existingFeedback), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingFeedback_ShouldDeleteFeedback()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var existingFeedback = FeedbackBuilder.Create()
                .WithUserId(userId)
                .WithCourseId(courseId)
                .Build();

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback);
            _mockFeedbackRepository.Setup(x => x.DeleteAsync(existingFeedback))
                .Returns(Task.CompletedTask);
            _mockFeedbackRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _feedbackService.DeleteAsync(userId, courseId);

            // Assert
            _mockFeedbackRepository.Verify(x => x.GetByUserAndCourseAsync(userId, courseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.DeleteAsync(existingFeedback), Times.Once);
            _mockFeedbackRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentFeedback_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync((Feedback?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.DeleteAsync(userId, courseId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            exception.Message.Should().Contain($"userId={userId}, courseId={courseId}");

            _mockFeedbackRepository.Verify(x => x.GetByUserAndCourseAsync(userId, courseId), Times.Once);
            _mockFeedbackRepository.Verify(x => x.DeleteAsync(It.IsAny<Feedback>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var existingFeedback = FeedbackBuilder.Create()
                .WithUserId(userId)
                .WithCourseId(courseId)
                .Build();

            _mockFeedbackRepository.Setup(x => x.GetByUserAndCourseAsync(userId, courseId))
                .ReturnsAsync(existingFeedback);
            _mockFeedbackRepository.Setup(x => x.DeleteAsync(existingFeedback))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.DeleteAsync(userId, courseId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("FEEDBACK_SERVICE_ERROR");
            exception.Message.Should().Contain("Error deleting feedback");

            _mockFeedbackRepository.Verify(x => x.DeleteAsync(existingFeedback), Times.Once);
        }

        #endregion

        #region GetCourseAverageRatingAsync Tests

        [Fact]
        public async Task GetCourseAverageRatingAsync_WithValidCourseId_ShouldReturnAverageRating()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedAverage = 4.2m;

            _mockFeedbackRepository.Setup(x => x.GetCourseAverageRatingAsync(courseId))
                .ReturnsAsync(expectedAverage);

            // Act
            var result = await _feedbackService.GetCourseAverageRatingAsync(courseId);

            // Assert
            result.Should().Be(expectedAverage);

            _mockFeedbackRepository.Verify(x => x.GetCourseAverageRatingAsync(courseId), Times.Once);
        }

        [Fact]
        public async Task GetCourseAverageRatingAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _mockFeedbackRepository.Setup(x => x.GetCourseAverageRatingAsync(courseId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _feedbackService.GetCourseAverageRatingAsync(courseId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("FEEDBACK_SERVICE_ERROR");
            exception.Message.Should().Contain("Error getting average rating");

            _mockFeedbackRepository.Verify(x => x.GetCourseAverageRatingAsync(courseId), Times.Once);
        }

        #endregion
    }
}
