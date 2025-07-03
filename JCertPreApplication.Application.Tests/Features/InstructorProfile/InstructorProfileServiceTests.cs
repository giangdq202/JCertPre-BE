using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.InstructorProfile;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Profile;
using Xunit;

namespace JCertPreApplication.Application.Tests.Features.InstructorProfile
{
    public class InstructorProfileServiceTests
    {
        private readonly Mock<IInstructorProfileRepository> _mockInstructorProfileRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly InstructorProfileService _instructorProfileService;

        public InstructorProfileServiceTests()
        {
            _mockInstructorProfileRepository = new Mock<IInstructorProfileRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _instructorProfileService = new InstructorProfileService(
                _mockInstructorProfileRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_ValidData_ReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User 
            { 
                Id = userId,
                Role = new Role { Name = "Instructor" }
            };
            var profileDto = new InstructorProfileDto
            {
                UserId = userId,
                Bio = "Test Bio",
                Specialization = "Test Specialization"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync((Domain.Entities.InstructorProfile)null);
            _mockInstructorProfileRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.InstructorProfile>()))
                .ReturnsAsync((Domain.Entities.InstructorProfile profile) => profile);

            // Act
            var result = await _instructorProfileService.CreateInstructorProfileAsync(profileDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profileDto.Bio, result.Bio);
            Assert.Equal(profileDto.Specialization, result.Specialization);
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var profileDto = new InstructorProfileDto
            {
                UserId = Guid.NewGuid(),
                Bio = "Test Bio"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(profileDto.UserId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _instructorProfileService.CreateInstructorProfileAsync(profileDto));
        }

        [Fact]
        public async Task CreateInstructorProfileAsync_ProfileAlreadyExists_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User 
            { 
                Id = userId,
                Role = new Role { Name = "Instructor" }
            };
            var profileDto = new InstructorProfileDto
            {
                UserId = userId,
                Bio = "Test Bio"
            };
            var existingProfile = new Domain.Entities.InstructorProfile { UserId = userId };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(existingProfile);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _instructorProfileService.CreateInstructorProfileAsync(profileDto));
        }

        [Fact]
        public async Task GetInstructorProfileAsync_ExistingProfile_ReturnsProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profile = new Domain.Entities.InstructorProfile 
            { 
                Id = Guid.NewGuid(),
                UserId = userId,
                Bio = "Test Bio",
                Specialization = "Test Specialization"
            };

            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(profile);

            // Act
            var result = await _instructorProfileService.GetInstructorProfileAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(profile.Bio, result.Bio);
            Assert.Equal(profile.Specialization, result.Specialization);
        }

        [Fact]
        public async Task GetInstructorProfileAsync_NonExistentProfile_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync((Domain.Entities.InstructorProfile)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _instructorProfileService.GetInstructorProfileAsync(userId));
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_ValidUpdate_ReturnsUpdatedProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingProfile = new Domain.Entities.InstructorProfile 
            { 
                Id = Guid.NewGuid(),
                UserId = userId,
                Bio = "Old Bio",
                Specialization = "Old Specialization"
            };
            var updateDto = new InstructorProfileDto
            {
                UserId = userId,
                Bio = "Updated Bio",
                Specialization = "Updated Specialization"
            };

            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(existingProfile);
            _mockInstructorProfileRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.InstructorProfile>()))
                .ReturnsAsync((Domain.Entities.InstructorProfile profile) => profile);

            // Act
            var result = await _instructorProfileService.UpdateInstructorProfileAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Bio, result.Bio);
            Assert.Equal(updateDto.Specialization, result.Specialization);
        }

        [Fact]
        public async Task UpdateInstructorProfileAsync_ProfileNotFound_ThrowsException()
        {
            // Arrange
            var updateDto = new InstructorProfileDto
            {
                UserId = Guid.NewGuid(),
                Bio = "Updated Bio"
            };

            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(updateDto.UserId))
                .ReturnsAsync((Domain.Entities.InstructorProfile)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _instructorProfileService.UpdateInstructorProfileAsync(updateDto));
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_ExistingProfile_Succeeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profile = new Domain.Entities.InstructorProfile { Id = Guid.NewGuid(), UserId = userId };

            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(profile);
            _mockInstructorProfileRepository.Setup(x => x.DeleteAsync(It.IsAny<Domain.Entities.InstructorProfile>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _instructorProfileService.DeleteInstructorProfileAsync(userId);
            _mockInstructorProfileRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.InstructorProfile>()), Times.Once);
        }

        [Fact]
        public async Task DeleteInstructorProfileAsync_NonExistentProfile_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockInstructorProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync((Domain.Entities.InstructorProfile)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _instructorProfileService.DeleteInstructorProfileAsync(userId));
        }
    }
} 