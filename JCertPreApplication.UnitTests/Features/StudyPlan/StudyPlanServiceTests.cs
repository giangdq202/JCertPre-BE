using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.StudyPlan;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.StudyPlan
{
    /// <summary>
    /// Unit tests for StudyPlanService
    /// Testing study plan management including CRUD operations, business rules, and error handling
    /// </summary>
    public class StudyPlanServiceTests
    {
        private readonly StudyPlanServiceFixture _fixture;
        private readonly StudyPlanService _studyPlanService;
        private readonly Mock<IStudyPlanRepository> _mockStudyPlanRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public StudyPlanServiceTests()
        {
            _fixture = new StudyPlanServiceFixture();
            _studyPlanService = _fixture.StudyPlanService;
            _mockStudyPlanRepository = _fixture.MockStudyPlanRepository;
            _mockUserRepository = _fixture.MockUserRepository;
        }

        #region GetStudyPlanByIdAsync Tests

        [Fact]
        public async Task GetStudyPlanByIdAsync_WithExistingId_ShouldReturnMappedDto()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studyPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithName("Test Plan")
                .WithDescription("Test Description")
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(studyPlan);

            // Act
            var result = await _studyPlanService.GetStudyPlanByIdAsync(planId);

            // Assert
            result.Should().NotBeNull();
            result.PlanId.Should().Be(studyPlan.planId);
            result.StudentId.Should().Be(studyPlan.studentId);
            result.CreatedByStaffId.Should().Be(studyPlan.createdByStaffId);
            result.PlanName.Should().Be(studyPlan.planName);
            result.Description.Should().Be(studyPlan.description);
            result.StartDate.Should().Be(studyPlan.startDate);
            result.EndDate.Should().Be(studyPlan.endDate);

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
        }

        [Fact]
        public async Task GetStudyPlanByIdAsync_WithNonExistingId_ShouldThrowNotFoundException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync((Domain.Entities.StudyPlan?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() => 
                _studyPlanService.GetStudyPlanByIdAsync(planId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("StudyPlan");
            exception.Message.Should().Contain(planId.ToString());

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
        }

        [Fact]
        public async Task GetStudyPlanByIdAsync_WithValidId_ShouldCallRepositoryOnce()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studyPlan = StudyPlanBuilder.Create().WithId(planId).Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(studyPlan);

            // Act
            await _studyPlanService.GetStudyPlanByIdAsync(planId);

            // Assert
            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.VerifyNoOtherCalls();
        }

        #endregion

        #region GetAllStudyPlansAsync Tests

        [Fact]
        public async Task GetAllStudyPlansAsync_WhenPlansExist_ShouldReturnMappedDtos()
        {
            // Arrange
            var studyPlans = StudyPlanBuilder.CreateList(3);

            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(studyPlans);

            // Act
            var result = await _studyPlanService.GetAllStudyPlansAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var resultList = result.ToList();
            for (int i = 0; i < studyPlans.Count; i++)
            {
                var expectedPlan = studyPlans[i];
                var actualDto = resultList.FirstOrDefault(r => r.PlanId == expectedPlan.planId);

                actualDto.Should().NotBeNull();
                actualDto!.StudentId.Should().Be(expectedPlan.studentId);
                actualDto.CreatedByStaffId.Should().Be(expectedPlan.createdByStaffId);
                actualDto.PlanName.Should().Be(expectedPlan.planName);
                actualDto.Description.Should().Be(expectedPlan.description);
                actualDto.StartDate.Should().Be(expectedPlan.startDate);
                actualDto.EndDate.Should().Be(expectedPlan.endDate);
            }

            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAllStudyPlansAsync_WhenNoPlans_ShouldReturnEmptyList()
        {
            // Arrange
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(new List<Domain.Entities.StudyPlan>());

            // Act
            var result = await _studyPlanService.GetAllStudyPlansAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetAllStudyPlansAsync_ShouldCallRepositoryOnce()
        {
            // Arrange
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(new List<Domain.Entities.StudyPlan>());

            // Act
            await _studyPlanService.GetAllStudyPlansAsync();

            // Assert
            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);
            _mockStudyPlanRepository.VerifyNoOtherCalls();
        }

        #endregion

        #region CreateStudyPlanAsync Tests

        [Fact]
        public async Task CreateStudyPlanAsync_WithValidData_ShouldCreateAndReturnDto()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(studentId).Build();
            var studyPlanDto = StudyPlanServiceFixture.ValidStudyPlanDto(studentId, staffId);

            var expectedStudyPlan = StudyPlanBuilder.Create()
                .WithStudentId(studentId)
                .WithStaffId(staffId)
                .WithName(studyPlanDto.PlanName)
                .WithDescription(studyPlanDto.Description)
                .WithStartDate(studyPlanDto.StartDate)
                .WithEndDate(studyPlanDto.EndDate)
                .Build();

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .ReturnsAsync(expectedStudyPlan);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _studyPlanService.CreateStudyPlanAsync(studyPlanDto);

            // Assert
            result.Should().NotBeNull();
            result.StudentId.Should().Be(studentId);
            result.CreatedByStaffId.Should().Be(staffId);
            result.PlanName.Should().Be(studyPlanDto.PlanName);
            result.Description.Should().Be(studyPlanDto.Description);
            result.StartDate.Should().Be(studyPlanDto.StartDate);
            result.EndDate.Should().Be(studyPlanDto.EndDate);

            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateStudyPlanAsync_WithNonExistentStudent_ShouldThrowNotFoundException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var studyPlanDto = StudyPlanServiceFixture.ValidStudyPlanDto(studentId);

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.CreateStudyPlanAsync(studyPlanDto));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("Student");
            exception.Message.Should().Contain(studentId.ToString());

            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Never);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateStudyPlanAsync_ShouldGenerateNewGuid()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(studentId).Build();
            var studyPlanDto = StudyPlanServiceFixture.ValidStudyPlanDto(studentId);

            Domain.Entities.StudyPlan? capturedStudyPlan = null;
            
            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Callback<Domain.Entities.StudyPlan>(sp => capturedStudyPlan = sp)
                .ReturnsAsync((Domain.Entities.StudyPlan sp) => sp);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.CreateStudyPlanAsync(studyPlanDto);

            // Assert
            capturedStudyPlan.Should().NotBeNull();
            capturedStudyPlan!.planId.Should().NotBeEmpty();
            capturedStudyPlan.planId.Should().NotBe(studyPlanDto.PlanId); // Should generate new GUID
        }

        [Fact]
        public async Task CreateStudyPlanAsync_ShouldCallSaveChanges()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(studentId).Build();
            var studyPlanDto = StudyPlanServiceFixture.ValidStudyPlanDto(studentId);
            var createdPlan = StudyPlanBuilder.Create().Build();

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .ReturnsAsync(createdPlan);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.CreateStudyPlanAsync(studyPlanDto);

            // Assert
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateStudyPlanAsync_ShouldMapAllPropertiesCorrectly()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(45);
            
            var student = UserBuilder.Create().WithId(studentId).Build();
            var studyPlanDto = new StudyPlanDto
            {
                StudentId = studentId,
                CreatedByStaffId = staffId,
                PlanName = "Comprehensive JLPT N2 Plan",
                Description = "Detailed study plan for JLPT N2 preparation",
                StartDate = startDate,
                EndDate = endDate
            };

            Domain.Entities.StudyPlan? capturedStudyPlan = null;

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Callback<Domain.Entities.StudyPlan>(sp => capturedStudyPlan = sp)
                .ReturnsAsync((Domain.Entities.StudyPlan sp) => sp);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.CreateStudyPlanAsync(studyPlanDto);

            // Assert
            capturedStudyPlan.Should().NotBeNull();
            capturedStudyPlan!.studentId.Should().Be(studentId);
            capturedStudyPlan.createdByStaffId.Should().Be(staffId);
            capturedStudyPlan.planName.Should().Be("Comprehensive JLPT N2 Plan");
            capturedStudyPlan.description.Should().Be("Detailed study plan for JLPT N2 preparation");
            capturedStudyPlan.startDate.Should().Be(startDate);
            capturedStudyPlan.endDate.Should().Be(endDate);
        }

        #endregion

        #region UpdateStudyPlanAsync Tests

        [Fact]
        public async Task UpdateStudyPlanAsync_WithValidData_ShouldUpdateAndReturnDto()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var existingPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithName("Original Plan")
                .WithDescription("Original Description")
                .Build();

            var updateDto = UpdateStudyPlanDtoBuilder.Create()
                .WithPlanName("Updated Plan")
                .WithDescription("Updated Description")
                .WithStudentId(null) // Don't update student
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(existingPlan);
            _mockStudyPlanRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _studyPlanService.UpdateStudyPlanAsync(planId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.PlanId.Should().Be(planId);
            result.PlanName.Should().Be("Updated Plan");
            result.Description.Should().Be("Updated Description");

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStudyPlanAsync_WithNonExistingPlan_ShouldThrowNotFoundException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var updateDto = UpdateStudyPlanDtoBuilder.Default();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync((Domain.Entities.StudyPlan?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.UpdateStudyPlanAsync(planId, updateDto));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("StudyPlan");
            exception.Message.Should().Contain(planId.ToString());

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Never);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateStudyPlanAsync_WithInvalidStudentId_ShouldThrowNotFoundException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var newStudentId = Guid.NewGuid();
            var existingPlan = StudyPlanBuilder.Create().WithId(planId).Build();
            var updateDto = UpdateStudyPlanDtoBuilder.Create()
                .WithStudentId(newStudentId)
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(existingPlan);
            _mockUserRepository.Setup(x => x.GetByIdAsync(newStudentId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.UpdateStudyPlanAsync(planId, updateDto));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("Student");
            exception.Message.Should().Contain(newStudentId.ToString());

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(newStudentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStudyPlanAsync_WithPartialData_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var originalName = "Original Plan";
            var originalDescription = "Original Description";
            var originalStartDate = DateTime.UtcNow.Date;
            var originalEndDate = DateTime.UtcNow.Date.AddDays(30);

            var existingPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithName(originalName)
                .WithDescription(originalDescription)
                .WithStartDate(originalStartDate)
                .WithEndDate(originalEndDate)
                .Build();

            var updateDto = UpdateStudyPlanDtoBuilder.Create()
                .WithPlanName("Updated Name Only")
                .WithDescription(null) // Don't update description
                .WithStartDate(null)   // Don't update start date
                .WithEndDate(null)     // Don't update end date
                .WithStudentId(null)   // Don't update student
                .Build();

            Domain.Entities.StudyPlan? capturedPlan = null;

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(existingPlan);
            _mockStudyPlanRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Callback<Domain.Entities.StudyPlan>(sp => capturedPlan = sp)
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _studyPlanService.UpdateStudyPlanAsync(planId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.PlanName.Should().Be("Updated Name Only");
            result.Description.Should().Be(originalDescription); // Should remain unchanged
            result.StartDate.Should().Be(originalStartDate);     // Should remain unchanged
            result.EndDate.Should().Be(originalEndDate);         // Should remain unchanged

            capturedPlan.Should().NotBeNull();
            capturedPlan!.planName.Should().Be("Updated Name Only");
            capturedPlan.description.Should().Be(originalDescription);
            capturedPlan.startDate.Should().Be(originalStartDate);
            capturedPlan.endDate.Should().Be(originalEndDate);
        }

        [Fact]
        public async Task UpdateStudyPlanAsync_WithSameStudentId_ShouldNotValidateStudent()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var existingPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithStudentId(studentId)
                .Build();

            var updateDto = UpdateStudyPlanDtoBuilder.Create()
                .WithStudentId(studentId) // Same student ID
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(existingPlan);
            _mockStudyPlanRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.UpdateStudyPlanAsync(planId, updateDto);

            // Assert
            _mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStudyPlanAsync_WithDifferentStudentId_ShouldValidateNewStudent()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var oldStudentId = Guid.NewGuid();
            var newStudentId = Guid.NewGuid();
            var newStudent = UserBuilder.Create().WithId(newStudentId).Build();

            var existingPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithStudentId(oldStudentId)
                .Build();

            var updateDto = UpdateStudyPlanDtoBuilder.Create()
                .WithStudentId(newStudentId)
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(existingPlan);
            _mockUserRepository.Setup(x => x.GetByIdAsync(newStudentId))
                .ReturnsAsync(newStudent);
            _mockStudyPlanRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _studyPlanService.UpdateStudyPlanAsync(planId, updateDto);

            // Assert
            result.StudentId.Should().Be(newStudentId);
            _mockUserRepository.Verify(x => x.GetByIdAsync(newStudentId), Times.Once);
        }

        [Fact]
        public async Task UpdateStudyPlanAsync_ShouldCallSaveChanges()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var existingPlan = StudyPlanBuilder.Create().WithId(planId).Build();
            var updateDto = UpdateStudyPlanDtoBuilder.Create()
                .WithStudentId(null) // Don't update student to avoid validation
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(existingPlan);
            _mockStudyPlanRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.StudyPlan>()))
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.UpdateStudyPlanAsync(planId, updateDto);

            // Assert
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region DeleteStudyPlanAsync Tests

        [Fact]
        public async Task DeleteStudyPlanAsync_WithExistingPlanAndNoItems_ShouldDeleteSuccessfully()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studyPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithEmptyItems() // No study plan items
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(studyPlan);
            _mockStudyPlanRepository.Setup(x => x.DeleteAsync(studyPlan))
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.DeleteStudyPlanAsync(planId);

            // Assert
            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.DeleteAsync(studyPlan), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteStudyPlanAsync_WithNonExistingPlan_ShouldThrowNotFoundException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync((Domain.Entities.StudyPlan?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.DeleteStudyPlanAsync(planId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("StudyPlan");
            exception.Message.Should().Contain(planId.ToString());

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Never);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteStudyPlanAsync_WithExistingItems_ShouldThrowBadRequestException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studyPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithOneItem() // Has study plan items
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(studyPlan);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.DeleteStudyPlanAsync(planId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Message.Should().Contain("Cannot delete study plan that has items");

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Never);
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteStudyPlanAsync_WithMultipleItems_ShouldThrowBadRequestException()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studyPlanItems = new List<StudyPlanItem>
            {
                new StudyPlanItem { itemId = Guid.NewGuid() },
                new StudyPlanItem { itemId = Guid.NewGuid() },
                new StudyPlanItem { itemId = Guid.NewGuid() }
            };

            var studyPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithItems(studyPlanItems)
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(studyPlan);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.DeleteStudyPlanAsync(planId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Message.Should().Contain("Cannot delete study plan that has items");

            _mockStudyPlanRepository.Verify(x => x.GetByIdAsync(planId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.StudyPlan>()), Times.Never);
        }

        [Fact]
        public async Task DeleteStudyPlanAsync_ShouldCallSaveChanges()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var studyPlan = StudyPlanBuilder.Create()
                .WithId(planId)
                .WithEmptyItems()
                .Build();

            _mockStudyPlanRepository.Setup(x => x.GetByIdAsync(planId))
                .ReturnsAsync(studyPlan);
            _mockStudyPlanRepository.Setup(x => x.DeleteAsync(studyPlan))
                .Returns(Task.CompletedTask);
            _mockStudyPlanRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _studyPlanService.DeleteStudyPlanAsync(planId);

            // Assert
            _mockStudyPlanRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region GetStudyPlansByStudentIdAsync Tests

        [Fact]
        public async Task GetStudyPlansByStudentIdAsync_WithValidStudentId_ShouldReturnFilteredPlans()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(studentId).Build();
            var mixedPlans = StudyPlanServiceFixture.CreateMixedStudyPlans(studentId);

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(mixedPlans);

            // Act
            var result = await _studyPlanService.GetStudyPlansByStudentIdAsync(studentId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(2); // Only 2 plans for target student
            resultList.Should().AllSatisfy(plan => plan.StudentId.Should().Be(studentId));

            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetStudyPlansByStudentIdAsync_WithNonExistentStudent_ShouldThrowNotFoundException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _studyPlanService.GetStudyPlansByStudentIdAsync(studentId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("Student");
            exception.Message.Should().Contain(studentId.ToString());

            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetStudyPlansByStudentIdAsync_WithNoPlansForStudent_ShouldReturnEmptyList()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(studentId).Build();
            var otherStudentPlans = StudyPlanBuilder.CreateList(3); // Plans for other students

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(otherStudentPlans);

            // Act
            var result = await _studyPlanService.GetStudyPlansByStudentIdAsync(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetStudyPlansByStudentIdAsync_ShouldOnlyReturnPlansForSpecificStudent()
        {
            // Arrange
            var targetStudentId = Guid.NewGuid();
            var otherStudentId1 = Guid.NewGuid();
            var otherStudentId2 = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(targetStudentId).Build();

            var allPlans = new List<Domain.Entities.StudyPlan>
            {
                // 3 plans for target student
                StudyPlanBuilder.Create().WithStudentId(targetStudentId).WithName("Target Plan 1").Build(),
                StudyPlanBuilder.Create().WithStudentId(targetStudentId).WithName("Target Plan 2").Build(),
                StudyPlanBuilder.Create().WithStudentId(targetStudentId).WithName("Target Plan 3").Build(),
                // 2 plans for other students
                StudyPlanBuilder.Create().WithStudentId(otherStudentId1).WithName("Other Plan 1").Build(),
                StudyPlanBuilder.Create().WithStudentId(otherStudentId2).WithName("Other Plan 2").Build()
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(targetStudentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(allPlans);

            // Act
            var result = await _studyPlanService.GetStudyPlansByStudentIdAsync(targetStudentId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(3);
            resultList.Should().AllSatisfy(plan => 
            {
                plan.StudentId.Should().Be(targetStudentId);
                plan.PlanName.Should().StartWith("Target Plan");
            });

            // Verify no plans for other students are returned
            resultList.Should().NotContain(plan => plan.StudentId == otherStudentId1);
            resultList.Should().NotContain(plan => plan.StudentId == otherStudentId2);
        }

        [Fact]
        public async Task GetStudyPlansByStudentIdAsync_WithValidStudent_ShouldValidateStudentFirst()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = UserBuilder.Create().WithId(studentId).Build();
            var plans = StudyPlanServiceFixture.CreateStudyPlansForStudent(studentId, 1);

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(plans);

            // Act
            await _studyPlanService.GetStudyPlansByStudentIdAsync(studentId);

            // Assert - Verify validation happens before fetching plans
            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockStudyPlanRepository.Verify(x => x.GetAllAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetStudyPlansByStudentIdAsync_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var staffId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(60);
            
            var student = UserBuilder.Create().WithId(studentId).Build();
            var studyPlan = StudyPlanBuilder.Create()
                .WithStudentId(studentId)
                .WithStaffId(staffId)
                .WithName("Detailed JLPT N1 Plan")
                .WithDescription("Advanced study plan for JLPT N1")
                .WithStartDate(startDate)
                .WithEndDate(endDate)
                .Build();

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockStudyPlanRepository.Setup(x => x.GetAllAsync((string?)null))
                .ReturnsAsync(new List<Domain.Entities.StudyPlan> { studyPlan });

            // Act
            var result = await _studyPlanService.GetStudyPlansByStudentIdAsync(studentId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);

            var dto = resultList.First();
            dto.PlanId.Should().Be(studyPlan.planId);
            dto.StudentId.Should().Be(studentId);
            dto.CreatedByStaffId.Should().Be(staffId);
            dto.PlanName.Should().Be("Detailed JLPT N1 Plan");
            dto.Description.Should().Be("Advanced study plan for JLPT N1");
            dto.StartDate.Should().Be(startDate);
            dto.EndDate.Should().Be(endDate);
        }

        #endregion
    }
}
