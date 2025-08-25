using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.StudentProfile;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using StudentProfileEntity = JCertPreApplication.Domain.Entities.StudentProfile;

namespace JCertPreApplication.UnitTests.Features.StudentProfile
{
    public class StudentProfileServiceTests
    {
        private readonly StudentProfileService _studentProfileService;
        private readonly Mock<IStudentProfileRepository> _mockRepository;

        public StudentProfileServiceTests()
        {
            _mockRepository = new Mock<IStudentProfileRepository>();
            _studentProfileService = new StudentProfileService(_mockRepository.Object);
        }

        #region CreateStudentProfileAsync Tests

        [Fact]
        public async Task CreateStudentProfileAsync_WithValidStudentUser_ShouldCreateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N3";
            var learningGoals = "Pass JLPT N2 within 6 months";
            var expectedProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel(currentLevel)
                .WithLearningGoals(learningGoals)
                .Build();

            _mockRepository.Setup(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.CurrentLevel.Should().Be(currentLevel);
            result.LearningGoals.Should().Be(learningGoals);

            _mockRepository.Verify(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        [Fact]
        public async Task CreateStudentProfileAsync_WithNonExistentUser_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N3";
            var learningGoals = "Pass JLPT N2";

            _mockRepository.Setup(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("STUDENT_PROFILE_CREATE_ERROR");
            apiException.Message.Should().Contain("Student profile creation failed");

            _mockRepository.Verify(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        [Fact]
        public async Task CreateStudentProfileAsync_WithNonStudentUser_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N3";
            var learningGoals = "Pass JLPT N2";

            _mockRepository.Setup(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("STUDENT_PROFILE_CREATE_ERROR");
            apiException.Message.Should().Contain("Student profile creation failed");

            _mockRepository.Verify(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        [Fact]
        public async Task CreateStudentProfileAsync_WithEmptyFields_ShouldCreateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "";
            var learningGoals = "";
            var expectedProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel(currentLevel)
                .WithLearningGoals(learningGoals)
                .Build();

            _mockRepository.Setup(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.CurrentLevel.Should().Be(currentLevel);
            result.LearningGoals.Should().Be(learningGoals);

            _mockRepository.Verify(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        [Fact]
        public async Task CreateStudentProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N3";
            var learningGoals = "Pass JLPT N2";
            var exceptionMessage = "Database connection failed";

            _mockRepository.Setup(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.CreateStudentProfileAsync(userId, currentLevel, learningGoals));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("STUDENT_PROFILE_SERVICE_ERROR");
            apiException.Message.Should().Contain(exceptionMessage);

            _mockRepository.Verify(x => x.CreateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        #endregion

        #region GetStudentProfileAsync Tests

        [Fact]
        public async Task GetStudentProfileAsync_WithExistingStudentProfile_ShouldReturnProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel("N3")
                .WithLearningGoals("Pass JLPT N2")
                .Build();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _studentProfileService.GetStudentProfileAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.CurrentLevel.Should().Be(expectedProfile.currentLevel);
            result.LearningGoals.Should().Be(expectedProfile.learningGoals);

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetStudentProfileAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.GetStudentProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("StudentProfile");

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetStudentProfileAsync_WithNonStudentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.GetStudentProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("StudentProfile");

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetStudentProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var exceptionMessage = "Database connection failed";

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.GetStudentProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("STUDENT_PROFILE_SERVICE_ERROR");
            apiException.Message.Should().Contain(exceptionMessage);

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
        }

        #endregion

        #region UpdateStudentProfileAsync Tests

        [Fact]
        public async Task UpdateStudentProfileAsync_WithValidData_ShouldUpdateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N2";
            var learningGoals = "Pass JLPT N1 within 1 year";
            var existingProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel("N3")
                .WithLearningGoals("Pass JLPT N2")
                .Build();
            var updatedProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel(currentLevel)
                .WithLearningGoals(learningGoals)
                .Build();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ReturnsAsync(updatedProfile);

            // Act
            var result = await _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.CurrentLevel.Should().Be(currentLevel);
            result.LearningGoals.Should().Be(learningGoals);

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        [Fact]
        public async Task UpdateStudentProfileAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N2";
            var learningGoals = "Pass JLPT N1";

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("StudentProfile");

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Never);
        }

        [Fact]
        public async Task UpdateStudentProfileAsync_WithNonStudentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N2";
            var learningGoals = "Pass JLPT N1";

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("StudentProfile");

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Never);
        }

        [Fact]
        public async Task UpdateStudentProfileAsync_WithPartialData_ShouldUpdateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N2";
            var learningGoals = "Pass JLPT N1";
            var existingProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel("N3")
                .WithLearningGoals("Pass JLPT N2")
                .Build();
            var updatedProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .WithCurrentLevel(currentLevel)
                .WithLearningGoals(learningGoals)
                .Build();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals))
                .ReturnsAsync(updatedProfile);

            // Act
            var result = await _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals);

            // Assert
            result.Should().NotBeNull();
            result.CurrentLevel.Should().Be(currentLevel);
            result.LearningGoals.Should().Be(learningGoals);

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Once);
        }

        [Fact]
        public async Task UpdateStudentProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentLevel = "N2";
            var learningGoals = "Pass JLPT N1";
            var exceptionMessage = "Database connection failed";

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.UpdateStudentProfileAsync(userId, currentLevel, learningGoals));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("STUDENT_PROFILE_UPDATE_ERROR");
            apiException.Message.Should().Contain(exceptionMessage);

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateStudentProfileAsync(userId, currentLevel, learningGoals), Times.Never);
        }

        #endregion

        #region DeleteStudentProfileAsync Tests

        [Fact]
        public async Task DeleteStudentProfileAsync_WithExistingProfile_ShouldDeleteSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingProfile = StudentProfileBuilder.Create()
                .WithUserId(userId)
                .Build();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.DeleteStudentProfileAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _studentProfileService.DeleteStudentProfileAsync(userId);

            // Assert
            result.Should().BeTrue();

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteStudentProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentProfileAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.DeleteStudentProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("StudentProfile");

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteStudentProfileAsync(userId), Times.Never);
        }

        [Fact]
        public async Task DeleteStudentProfileAsync_WithNonStudentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ReturnsAsync((StudentProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.DeleteStudentProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("StudentProfile");

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteStudentProfileAsync(userId), Times.Never);
        }

        [Fact]
        public async Task DeleteStudentProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var exceptionMessage = "Database connection failed";

            _mockRepository.Setup(x => x.ReadStudentProfileAsync(userId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _studentProfileService.DeleteStudentProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("STUDENT_PROFILE_DELETE_ERROR");
            apiException.Message.Should().Contain(exceptionMessage);

            _mockRepository.Verify(x => x.ReadStudentProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteStudentProfileAsync(userId), Times.Never);
        }

        #endregion
    }
}
