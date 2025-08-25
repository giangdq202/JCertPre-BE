using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using InstructorProfileEntity = JCertPreApplication.Domain.Entities.InstructorProfile;

namespace JCertPreApplication.UnitTests.Features.InstructorProfile
{
    public class InstructorProfileServiceTests
    {
        private readonly InstructorProfileService _instructorProfileService;
        private readonly Mock<IInstructorProfileRepository> _mockRepository;

        public InstructorProfileServiceTests()
        {
            _mockRepository = new Mock<IInstructorProfileRepository>();
            _instructorProfileService = new InstructorProfileService(_mockRepository.Object);
        }

        #region CreateInstructorProfileAsync Tests

        [Fact]
        public async Task CreateInstructorProfileAsync_WithValidInstructorUser_ShouldCreateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Experienced Japanese language instructor";
            var experience = "5+ years teaching experience";
            var teachingStyle = "Interactive approach";

            var expectedProfile = InstructorProfileBuilder.Create()
                .WithId(userId)
                .WithIntroduction(introduction)
                .WithExperience(experience)
                .WithTeachingStyle(teachingStyle)
                .Build();

            _mockRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Introduction.Should().Be(introduction);
            result.Experience.Should().Be(experience);
            result.TeachingStyle.Should().Be(teachingStyle);

            _mockRepository.Verify(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_WithNonExistentUser_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Experienced instructor";
            var experience = "5+ years experience";
            var teachingStyle = "Interactive approach";

            _mockRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("INSTRUCTOR_PROFILE_CREATE_ERROR");
            apiException.Message.Should().Contain("Instructor profile creation failed");

            _mockRepository.Verify(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_WithNonInstructorUser_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Student profile";
            var experience = "Learning experience";
            var teachingStyle = "Learning style";

            _mockRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("INSTRUCTOR_PROFILE_CREATE_ERROR");
            apiException.Message.Should().Contain("Instructor profile creation failed");

            _mockRepository.Verify(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_WithEmptyIntroduction_ShouldCreateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "";
            var experience = "5+ years experience";
            var teachingStyle = "Interactive approach";

            var expectedProfile = InstructorProfileBuilder.Create()
                .WithId(userId)
                .WithIntroduction(introduction)
                .WithExperience(experience)
                .WithTeachingStyle(teachingStyle)
                .Build();

            _mockRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);

            // Assert
            result.Should().NotBeNull();
            result.Introduction.Should().Be(introduction);

            _mockRepository.Verify(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Experienced instructor";
            var experience = "5+ years experience";
            var teachingStyle = "Interactive approach";

            var exception = new Exception("Database connection failed");
            _mockRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ThrowsAsync(exception);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("INSTRUCTOR_PROFILE_SERVICE_ERROR");
            apiException.Message.Should().Contain("An error occurred while creating instructor profile");

            _mockRepository.Verify(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        #endregion

        #region GetInstructorProfileAsync Tests

        [Fact]
        public async Task GetInstructorProfileAsync_WithExistingInstructorProfile_ShouldReturnProfile()
        {
            // Arrange
            var expectedProfile = InstructorProfileBuilder.Create().Build();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(expectedProfile.userId))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _instructorProfileService.GetInstructorProfileAsync(expectedProfile.userId);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(expectedProfile.userId);
            result.Introduction.Should().Be(expectedProfile.introduction);
            result.Experience.Should().Be(expectedProfile.experience);
            result.TeachingStyle.Should().Be(expectedProfile.teachingStyle);

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(expectedProfile.userId), Times.Once);
        }

        [Fact]
        public async Task GetInstructorProfileAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.GetInstructorProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("InstructorProfile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetInstructorProfileAsync_WithNonInstructorUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.GetInstructorProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetInstructorProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var exception = new Exception("Database connection failed");

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ThrowsAsync(exception);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.GetInstructorProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("INSTRUCTOR_PROFILE_SERVICE_ERROR");
            apiException.Message.Should().Contain("An error occurred while retrieving instructor profile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
        }

        #endregion

        #region UpdateInstructorProfileAsync Tests

        [Fact]
        public async Task UpdateInstructorProfileAsync_WithValidData_ShouldUpdateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated introduction";
            var experience = "Updated experience";
            var teachingStyle = "Updated teaching style";

            var existingProfile = InstructorProfileBuilder.Create().WithId(userId).Build();
            var updatedProfile = InstructorProfileBuilder.Create()
                .WithId(userId)
                .WithIntroduction(introduction)
                .WithExperience(experience)
                .WithTeachingStyle(teachingStyle)
                .Build();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync(updatedProfile);

            // Act
            var result = await _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.Introduction.Should().Be(introduction);
            result.Experience.Should().Be(experience);
            result.TeachingStyle.Should().Be(teachingStyle);

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated introduction";
            var experience = "Updated experience";
            var teachingStyle = "Updated teaching style";

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("InstructorProfile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Never);
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_WithNonInstructorUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated introduction";
            var experience = "Updated experience";
            var teachingStyle = "Updated teaching style";

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Never);
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_WithPartialData_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated introduction";
            string? experience = null;
            string? teachingStyle = null;

            var existingProfile = InstructorProfileBuilder.Create().WithId(userId).Build();
            var updatedProfile = InstructorProfileBuilder.Create()
                .WithId(userId)
                .WithIntroduction(introduction)
                .WithEmptyExperience()
                .WithEmptyTeachingStyle()
                .Build();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync(updatedProfile);

            // Act
            var result = await _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);

            // Assert
            result.Should().NotBeNull();
            result.Introduction.Should().Be(introduction);
            result.Experience.Should().BeNull();
            result.TeachingStyle.Should().BeNull();

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated introduction";
            var experience = "Updated experience";
            var teachingStyle = "Updated teaching style";

            var existingProfile = InstructorProfileBuilder.Create().WithId(userId).Build();
            var exception = new Exception("Database connection failed");

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ThrowsAsync(exception);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("INSTRUCTOR_PROFILE_UPDATE_ERROR");
            apiException.Message.Should().Contain("An error occurred while updating instructor profile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle), Times.Once);
        }

        #endregion

        #region DeleteInstructorProfileAsync Tests

        [Fact]
        public async Task DeleteInstructorProfileAsync_WithExistingProfile_ShouldDeleteSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingProfile = InstructorProfileBuilder.Create().WithId(userId).Build();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.DeleteInstructorProfileAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _instructorProfileService.DeleteInstructorProfileAsync(userId);

            // Assert
            result.Should().BeTrue();

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteInstructorProfileAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.DeleteInstructorProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("InstructorProfile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteInstructorProfileAsync(userId), Times.Never);
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_WithNonInstructorUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((InstructorProfileEntity?)null);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.DeleteInstructorProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.NotFound);
            apiException.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            apiException.Message.Should().Contain("InstructorProfile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteInstructorProfileAsync(userId), Times.Never);
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingProfile = InstructorProfileBuilder.Create().WithId(userId).Build();
            var exception = new Exception("Database connection failed");

            _mockRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockRepository.Setup(x => x.DeleteInstructorProfileAsync(userId))
                .ThrowsAsync(exception);

            // Act & Assert
            var apiException = await Assert.ThrowsAsync<ApiException>(
                () => _instructorProfileService.DeleteInstructorProfileAsync(userId));

            apiException.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            apiException.ErrorCode.Should().Be("INSTRUCTOR_PROFILE_DELETE_ERROR");
            apiException.Message.Should().Contain("An error occurred while deleting instructor profile");

            _mockRepository.Verify(x => x.ReadInstructorProfileAsync(userId), Times.Once);
            _mockRepository.Verify(x => x.DeleteInstructorProfileAsync(userId), Times.Once);
        }

        #endregion
    }
}
