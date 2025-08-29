using FluentAssertions;
using JCertPreApplication.Application.Dtos.TestTemplateType;
using JCertPreApplication.Application.Dtos.TestTemplateTypes;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.TestTemplateTypes
{
    public class TestTemplateTypeServiceTests : IClassFixture<TestTemplateTypeServiceFixture>
    {
        private readonly TestTemplateTypeServiceFixture _fixture;

        public TestTemplateTypeServiceTests(TestTemplateTypeServiceFixture fixture)
        {
            _fixture = fixture;
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithValidParameters_ShouldReturnPaginatedResults()
        {
            // Arrange
            var createdByUser = UserBuilder.Create().WithName("Creator User").Build();
            var verifiedByUser = UserBuilder.Create().WithName("Verifier User").Build();

            var types = new List<TestTemplateType>
            {
                TestTemplateTypeBuilder.Create()
                    .WithTypeName("JLPT N5 Grammar")
                    .WithCourseLevel(CourseLevel.N5)
                    .WithTestType(TestType.JLPTAuto)
                    .AsActive()
                    .WithCreatedByUser(createdByUser)
                    .WithVerifiedByUser(verifiedByUser)
                    .Build(),
                TestTemplateTypeBuilder.Create()
                    .WithTypeName("JLPT N4 Reading")
                    .WithCourseLevel(CourseLevel.N4)
                    .WithTestType(TestType.CustomManual)
                    .AsInactive()
                    .WithCreatedByUser(createdByUser)
                    .Build()
            };

            _fixture.SetupPaginationResults(2, types);

            // Act
            var result = await _fixture.TestTemplateTypeService.GetAllAsync(
                "JLPT", CourseLevel.N5, TestType.JLPTAuto, true, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.PageIndex.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalItemsCount.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items.Should().Contain(x => x.typeName == "JLPT N5 Grammar");
            result.Items.Should().Contain(x => x.CreatedByUserName == "Creator User");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetAllAsync_WithSearchFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var types = new List<TestTemplateType>
            {
                TestTemplateTypeBuilder.Create()
                    .WithTypeName("Grammar Test")
                    .Build()
            };

            _fixture.SetupPaginationResults(1, types);

            // Act
            var result = await _fixture.TestTemplateTypeService.GetAllAsync(
                "Grammar", null, null, null, 1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().typeName.Should().Be("Grammar Test");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetPaginationAsync(
                    It.IsAny<Expression<Func<TestTemplateType, bool>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Func<IQueryable<TestTemplateType>, IOrderedQueryable<TestTemplateType>>>()
                ))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.GetAllAsync(
                null, null, null, null, 1, 10);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("GET_TEST_TEMPLATE_TYPE_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateType()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createDto = TestTemplateTypeServiceFixture.ValidCreateDto(userId);

            _fixture.SetupNoDuplicateScenario(createDto.testType, createDto.courseLevel);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.InsertAsync(It.IsAny<TestTemplateType>()))
                .ReturnsAsync(It.IsAny<TestTemplateType>());

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _fixture.TestTemplateTypeService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.userId.Should().Be(userId);
            result.typeName.Should().Be(createDto.typeName);
            result.courseLevel.Should().Be(createDto.courseLevel);
            result.testType.Should().Be(createDto.testType);
            result.description.Should().Be(createDto.description);
            result.isActive.Should().BeFalse(); // New types start inactive
            result.verifiedUserId.Should().BeNull(); // New types start unverified

            _fixture.MockTestTemplateTypeRepository.Verify(x => x.InsertAsync(It.IsAny<TestTemplateType>()), Times.Once);
            _fixture.MockTestTemplateTypeRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateTypeAndLevel_ShouldThrowBadRequestException()
        {
            // Arrange
            var createDto = TestTemplateTypeServiceFixture.ValidCreateDto();

            _fixture.SetupDuplicateScenario(createDto.testType, createDto.courseLevel);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.CreateAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("DUPLICATE_TEST_TEMPLATE_TYPE");
            exception.Which.Message.Should().Contain("same test type and course level already exists");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var createDto = TestTemplateTypeServiceFixture.ValidCreateDto();

            _fixture.SetupNoDuplicateScenario(createDto.testType, createDto.courseLevel);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.InsertAsync(It.IsAny<TestTemplateType>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.CreateAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("CREATE_TEST_TEMPLATE_TYPE_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateType()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var updateDto = TestTemplateTypeServiceFixture.ValidUpdateDto();

            var existingType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .WithTypeName("Original Name")
                .WithCourseLevel(CourseLevel.N5)
                .WithTestType(TestType.JLPTAuto)
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(existingType);

            // Setup no duplicate for new values
            _fixture.SetupNoDuplicateScenario(updateDto.testType!.Value, updateDto.courseLevel!.Value);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.UpdateAsync(It.IsAny<TestTemplateType>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _fixture.TestTemplateTypeService.UpdateAsync(typeId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.TestTemplateTypeId.Should().Be(typeId);
            result.typeName.Should().Be(updateDto.typeName);
            result.courseLevel.Should().Be(updateDto.courseLevel!.Value);
            result.testType.Should().Be(updateDto.testType!.Value);
            result.description.Should().Be(updateDto.description);
            result.isActive.Should().BeFalse(); // Service always sets isActive to false when updating

            _fixture.MockTestTemplateTypeRepository.Verify(x => x.UpdateAsync(It.IsAny<TestTemplateType>()), Times.Once);
            _fixture.MockTestTemplateTypeRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentType_ShouldThrowNotFoundException()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var updateDto = TestTemplateTypeServiceFixture.ValidUpdateDto();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync((TestTemplateType?)null);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.UpdateAsync(typeId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Which.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateAsync_WithDuplicateTypeAndLevel_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var updateDto = TestTemplateTypeServiceFixture.ValidUpdateDto();

            var existingType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(existingType);

            // Setup duplicate scenario for new values
            _fixture.SetupDuplicateScenario(updateDto.testType!.Value, updateDto.courseLevel!.Value);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.UpdateAsync(typeId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("DUPLICATE_TEST_TEMPLATE_TYPE");

            _fixture.ResetMocks();
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithInactiveType_ShouldDeleteType()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            var inactiveType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsInactive()
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(inactiveType);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.DeleteAsync(It.IsAny<TestTemplateType>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _fixture.TestTemplateTypeService.DeleteAsync(typeId);

            // Assert
            _fixture.MockTestTemplateTypeRepository.Verify(x => x.DeleteAsync(inactiveType), Times.Once);
            _fixture.MockTestTemplateTypeRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task DeleteAsync_WithActiveTypeUsedInOpenTest_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.SetupActiveTypeWithOpenTests(typeId);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.DeleteAsync(typeId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
            exception.Which.Message.Should().Contain("Cannot delete an active template type that is used in any open test");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentType_ShouldThrowNotFoundException()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync((TestTemplateType?)null);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.DeleteAsync(typeId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Which.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _fixture.ResetMocks();
        }

        #endregion

        #region UpdateIsActiveAsync Tests

        [Fact]
        public async Task UpdateIsActiveAsync_WithValidActivation_ShouldActivateType()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.SetupVerifiedTypeWithTemplatesAndConfigs(typeId);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.UpdateAsync(It.IsAny<TestTemplateType>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _fixture.TestTemplateTypeService.UpdateIsActiveAsync(typeId, true);

            // Assert
            result.Should().NotBeNull();
            result.TestTemplateTypeId.Should().Be(typeId);
            result.isActive.Should().BeTrue();

            _fixture.MockTestTemplateTypeRepository.Verify(x => x.UpdateAsync(It.IsAny<TestTemplateType>()), Times.Once);
            _fixture.MockTestTemplateTypeRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateIsActiveAsync_WithUnverifiedType_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.SetupUnverifiedType(typeId);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.UpdateIsActiveAsync(typeId, true);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("NOT_VERIFIED");
            exception.Which.Message.Should().Contain("This template type is not verified");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateIsActiveAsync_WithNoTestTemplates_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.SetupTypeWithoutTemplates(typeId);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.UpdateIsActiveAsync(typeId, true);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("NO_TEST_TEMPLATE");
            exception.Which.Message.Should().Contain("No test template belongs to this type");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateIsActiveAsync_WithNoTestTemplateConfigs_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.SetupTypeWithoutConfigs(typeId);

            // Act
            var act = async () => await _fixture.TestTemplateTypeService.UpdateIsActiveAsync(typeId, true);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("NO_TEST_TEMPLATE_CONFIG");
            exception.Which.Message.Should().Contain("No test template config belongs to any test template of this type");

            _fixture.ResetMocks();
        }

        #endregion

        #region VerifyAsync Tests

        [Fact]
        public async Task VerifyAsync_WithValidData_ShouldVerifyType()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();
            var verifierId = Guid.NewGuid(); // Different from creator

            var existingType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .WithUserId(creatorId)
                .AsUnverified()
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(existingType);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.UpdateAsync(It.IsAny<TestTemplateType>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _fixture.TestTemplateTypeService.VerifyAsync(typeId, verifierId);

            // Assert
            result.Should().NotBeNull();
            result.TestTemplateTypeId.Should().Be(typeId);
            result.verifiedUserId.Should().Be(verifierId);

            _fixture.MockTestTemplateTypeRepository.Verify(x => x.UpdateAsync(It.IsAny<TestTemplateType>()), Times.Once);
            _fixture.MockTestTemplateTypeRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task VerifyAsync_WithSelfVerification_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var creatorId = Guid.NewGuid();

            var existingType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .WithUserId(creatorId) // Creator
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(existingType);

            // Act - Try to verify with same user ID
            var act = async () => await _fixture.TestTemplateTypeService.VerifyAsync(typeId, creatorId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("SELF_VERIFY_NOT_ALLOWED");
            exception.Which.Message.Should().Contain("The creator cannot verify their own template type");

            _fixture.ResetMocks();
        }

        #endregion

        #region GetTemplateTypeSummaryAsync Tests

        [Fact]
        public async Task GetTemplateTypeSummaryAsync_WithValidData_ShouldReturnSummary()
        {
            // Arrange
            var courseLevel = CourseLevel.N5;
            var testType = TestType.JLPTAuto;

            var templateType = TestTemplateTypeBuilder.Create()
                .WithCourseLevel(courseLevel)
                .WithTestType(testType)
                .WithTypeName("N5 Auto Test")
                .WithTotalTestScore(100)
                .WithTotalPassPercentage(70m)
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(templateType);

            // Setup templates
            var templates = new List<TestTemplate>
            {
                TestTemplateBuilder.Create()
                    .WithId(Guid.NewGuid())
                    .WithTypeId(templateType.TestTemplateTypeId)
                    .WithName("Grammar Template")
                    .WithScore(50)
                    .WithPassPercentage(65m)
                    .WithDuration(60)
                    .Build(),
                TestTemplateBuilder.Create()
                    .WithId(Guid.NewGuid())
                    .WithTypeId(templateType.TestTemplateTypeId)
                    .WithName("Reading Template")
                    .WithScore(50)
                    .WithPassPercentage(75m)
                    .WithDuration(90)
                    .Build()
            };

            var templatesQueryable = templates.AsQueryable();
            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(templatesQueryable);

            // Setup configs
            var configs = new List<TestTemplateConfig>
            {
                TestTemplateConfigBuilder.Create()
                    .WithTemplateId(templates[0].templateId)
                    .WithQuestionCount(10)
                    .Build(),
                TestTemplateConfigBuilder.Create()
                    .WithTemplateId(templates[0].templateId)
                    .WithQuestionCount(15)
                    .Build(),
                TestTemplateConfigBuilder.Create()
                    .WithTemplateId(templates[1].templateId)
                    .WithQuestionCount(20)
                    .Build()
            };

            var configsQueryable = configs.AsQueryable();
            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetAll())
                .ReturnsAsync(configsQueryable);

            // Act
            var result = await _fixture.TestTemplateTypeService.GetTemplateTypeSummaryAsync(courseLevel, testType);

            // Assert
            result.Should().NotBeNull();
            result!.TestTemplateTypeId.Should().Be(templateType.TestTemplateTypeId);
            result.TypeName.Should().Be("N5 Auto Test");
            result.CourseLevel.Should().Be(courseLevel);
            result.TestType.Should().Be(testType);
            result.TotalTestScore.Should().Be(100);
            result.TotalPassPercentage.Should().Be(70m);
            result.TotalDurationMinutes.Should().Be(150); // 60 + 90
            result.TestTemplates.Should().HaveCount(2);
            result.TestTemplates.Should().Contain(x => x.TemplateName == "Grammar Template" && x.TotalQuestionCount == 25); // 10 + 15
            result.TestTemplates.Should().Contain(x => x.TemplateName == "Reading Template" && x.TotalQuestionCount == 20);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetTemplateTypeSummaryAsync_WithNonExistentType_ShouldReturnNull()
        {
            // Arrange
            var courseLevel = CourseLevel.N5;
            var testType = TestType.JLPTAuto;

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<TestTemplateType, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((TestTemplateType?)null);

            // Act
            var result = await _fixture.TestTemplateTypeService.GetTemplateTypeSummaryAsync(courseLevel, testType);

            // Assert
            result.Should().BeNull();

            _fixture.ResetMocks();
        }

        #endregion
    }
}
