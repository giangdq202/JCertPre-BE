using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Application.Exceptions;
using Xunit;

namespace JCertPreApplication.Application.Tests.Features.InstructorProfile
{
    public class InstructorProfileServiceTests
    {
        private readonly Mock<IInstructorProfileRepository> _mockInstructorProfileRepository;
        private readonly InstructorProfileService _instructorProfileService;

        public InstructorProfileServiceTests()
        {
            _mockInstructorProfileRepository = new Mock<IInstructorProfileRepository>();
            _instructorProfileService = new InstructorProfileService(_mockInstructorProfileRepository.Object);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_ValidData_ReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Test Introduction";
            var experience = "Test Experience";
            var teachingStyle = "Test Style";

            var profile = new Domain.Entities.InstructorProfile
            {
                userId = userId,
                introduction = introduction,
                experience = experience,
                teachingStyle = teachingStyle
            };

            _mockInstructorProfileRepository.Setup(x => x.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync(profile);

            // Act
            var result = await _instructorProfileService.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(introduction, result.Introduction);
            Assert.Equal(experience, result.Experience);
            Assert.Equal(teachingStyle, result.TeachingStyle);
        }

        [Fact]
        public async Task GetInstructorProfileAsync_ExistingProfile_ReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profile = new Domain.Entities.InstructorProfile
            {
                userId = userId,
                introduction = "Test Introduction",
                experience = "Test Experience",
                teachingStyle = "Test Style"
            };

            _mockInstructorProfileRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(profile);

            // Act
            var result = await _instructorProfileService.GetInstructorProfileAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profile.introduction, result.Introduction);
            Assert.Equal(profile.experience, result.Experience);
            Assert.Equal(profile.teachingStyle, result.TeachingStyle);
        }

        [Fact]
        public async Task GetInstructorProfileAsync_NonExistentProfile_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockInstructorProfileRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((Domain.Entities.InstructorProfile?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => 
                _instructorProfileService.GetInstructorProfileAsync(userId));
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_ValidUpdate_ReturnsUpdatedProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated Introduction";
            var experience = "Updated Experience";
            var teachingStyle = "Updated Style";

            var existingProfile = new Domain.Entities.InstructorProfile
            {
                userId = userId,
                introduction = "Old Introduction",
                experience = "Old Experience",
                teachingStyle = "Old Style"
            };

            _mockInstructorProfileRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(existingProfile);

            var updatedProfile = new Domain.Entities.InstructorProfile
            {
                userId = userId,
                introduction = introduction,
                experience = experience,
                teachingStyle = teachingStyle
            };

            _mockInstructorProfileRepository.Setup(x => x.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle))
                .ReturnsAsync(updatedProfile);

            // Act
            var result = await _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(introduction, result.Introduction);
            Assert.Equal(experience, result.Experience);
            Assert.Equal(teachingStyle, result.TeachingStyle);
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_ProfileNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var introduction = "Updated Introduction";
            var experience = "Updated Experience";
            var teachingStyle = "Updated Style";

            _mockInstructorProfileRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((Domain.Entities.InstructorProfile?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => 
                _instructorProfileService.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle));
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_ExistingProfile_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profile = new Domain.Entities.InstructorProfile { userId = userId };

            _mockInstructorProfileRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync(profile);
            _mockInstructorProfileRepository.Setup(x => x.DeleteInstructorProfileAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _instructorProfileService.DeleteInstructorProfileAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_NonExistentProfile_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockInstructorProfileRepository.Setup(x => x.ReadInstructorProfileAsync(userId))
                .ReturnsAsync((Domain.Entities.InstructorProfile?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => 
                _instructorProfileService.DeleteInstructorProfileAsync(userId));
        }
    }
} 