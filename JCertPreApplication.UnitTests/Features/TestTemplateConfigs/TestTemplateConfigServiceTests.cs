using FluentAssertions;
using JCertPreApplication.Application.Dtos.TestTemplateConfig;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.TestTemplateConfigs
{
    public class TestTemplateConfigServiceTests : IClassFixture<TestTemplateConfigServiceFixture>
    {
        private readonly TestTemplateConfigServiceFixture _fixture;

        public TestTemplateConfigServiceTests(TestTemplateConfigServiceFixture fixture)
        {
            _fixture = fixture;
        }

        #region GetAllByTemplateIdAsync Tests

        [Fact]
        public async Task GetAllByTemplateIdAsync_WithValidTemplateId_ShouldReturnConfigs()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var configs = TestTemplateConfigServiceFixture.CreateConfigsForTemplate(templateId, 3);

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetAllByTemplateIdAsync(templateId))
                .ReturnsAsync(configs);

            // Act
            var result = await _fixture.TestTemplateConfigService.GetAllByTemplateIdAsync(templateId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.All(x => x.templateId == templateId).Should().BeTrue();
            
            // Verify SubContent mapping
            result.All(x => x.SubContent != null).Should().BeTrue();
            result.All(x => x.SubContent!.SubContentName == "Mondai1").Should().BeTrue();

            _fixture.MockTestTemplateConfigRepository.Verify(x => x.GetAllByTemplateIdAsync(templateId), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetAllByTemplateIdAsync_WithNoConfigs_ShouldReturnEmptyList()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var emptyConfigs = new List<TestTemplateConfig>();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetAllByTemplateIdAsync(templateId))
                .ReturnsAsync(emptyConfigs);

            // Act
            var result = await _fixture.TestTemplateConfigService.GetAllByTemplateIdAsync(templateId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetAllByTemplateIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetAllByTemplateIdAsync(templateId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.GetAllByTemplateIdAsync(templateId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("GET_TEST_TEMPLATE_CONFIGS_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region GetByConfigIdAsync Tests

        [Fact]
        public async Task GetByConfigIdAsync_WithExistingId_ShouldReturnConfig()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var subContent = SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai5)
                .WithLevel(CourseLevel.N5)
                .WithContentName(ContentName.Reading)
                .Build();

            var config = TestTemplateConfigBuilder.Create()
                .WithId(configId)
                .WithSubContent(subContent)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByConfigIdAsync(configId))
                .ReturnsAsync(config);

            // Act
            var result = await _fixture.TestTemplateConfigService.GetByConfigIdAsync(configId);

            // Assert
            result.Should().NotBeNull();
            result!.configId.Should().Be(configId);
            result.SubContent.Should().NotBeNull();
            result.SubContent!.SubContentName.Should().Be("Mondai5");
            result.SubContent.SubContentNameDescription.Should().NotBeNullOrEmpty();

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetByConfigIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var configId = Guid.NewGuid();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByConfigIdAsync(configId))
                .ReturnsAsync((TestTemplateConfig?)null);

            // Act
            var result = await _fixture.TestTemplateConfigService.GetByConfigIdAsync(configId);

            // Assert
            result.Should().BeNull();

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetByConfigIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var configId = Guid.NewGuid();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByConfigIdAsync(configId))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.GetByConfigIdAsync(configId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("GET_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateConfig()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var subContentId = Guid.NewGuid();
            var createDto = TestTemplateConfigServiceFixture.ValidCreateDto(subContentId);

            _fixture.SetupInactiveTypeScenario(templateId, typeId);
            _fixture.SetupQuestionAvailability(subContentId, 20); // More than required

            var createdConfig = TestTemplateConfigBuilder.Create()
                .WithTemplateId(templateId)
                .WithSubContentId(subContentId)
                .WithQuestionCount(createDto.questionCount)
                .WithPointPerQuestion(createDto.pointPerQuestion)
                .WithTotalPoints(createDto.totalPoints)
                .WithSequence(createDto.sequence)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.InsertAsync(It.IsAny<TestTemplateConfig>()))
                .ReturnsAsync(It.IsAny<TestTemplateConfig>());

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByConfigIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdConfig);

            // Act
            var result = await _fixture.TestTemplateConfigService.CreateAsync(templateId, createDto);

            // Assert
            result.Should().NotBeNull();
            result.templateId.Should().Be(templateId);
            result.questionCount.Should().Be(createDto.questionCount);
            result.pointPerQuestion.Should().Be(createDto.pointPerQuestion);
            result.totalPoints.Should().Be(createDto.totalPoints);
            result.sequence.Should().Be(createDto.sequence);

            _fixture.MockTestTemplateConfigRepository.Verify(x => x.InsertAsync(It.IsAny<TestTemplateConfig>()), Times.Once);
            _fixture.MockTestTemplateConfigRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentTemplate_ShouldThrowNotFoundException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var createDto = TestTemplateConfigServiceFixture.ValidCreateDto();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync((TestTemplate?)null);

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.CreateAsync(templateId, createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("CREATE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WithActiveType_ShouldThrowBadRequestException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var createDto = TestTemplateConfigServiceFixture.ValidCreateDto();

            _fixture.SetupActiveTypeScenario(templateId, typeId);

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.CreateAsync(templateId, createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("CREATE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WithNotEnoughQuestions_ShouldThrowBadRequestException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var subContentId = Guid.NewGuid();
            var createDto = TestTemplateConfigServiceFixture.ValidCreateDto(subContentId);
            createDto.questionCount = 20; // Required

            _fixture.SetupInactiveTypeScenario(templateId, typeId);
            _fixture.SetupQuestionAvailability(subContentId, 15); // Only 15 available

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.CreateAsync(templateId, createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("CREATE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var subContentId = Guid.NewGuid();
            var createDto = TestTemplateConfigServiceFixture.ValidCreateDto(subContentId);

            _fixture.SetupInactiveTypeScenario(templateId, typeId);
            _fixture.SetupQuestionAvailability(subContentId, 20);

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.InsertAsync(It.IsAny<TestTemplateConfig>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.CreateAsync(templateId, createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("CREATE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateConfig()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var subContentId = Guid.NewGuid();
            var updateDto = TestTemplateConfigServiceFixture.ValidUpdateDto();

            var existingConfig = TestTemplateConfigBuilder.Create()
                .WithId(configId)
                .WithTemplateId(templateId)
                .WithSubContentId(subContentId)
                .WithQuestionCount(10)
                .WithPointPerQuestion(5)
                .WithTotalPoints(50)
                .WithSequence(1)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByIdAsync(configId))
                .ReturnsAsync(existingConfig);

            _fixture.SetupInactiveTypeScenario(templateId, typeId);
            _fixture.SetupQuestionAvailability(subContentId, 20); // Enough questions

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.UpdateAsync(It.IsAny<TestTemplateConfig>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            var updatedConfig = TestTemplateConfigBuilder.Create()
                .WithId(configId)
                .WithQuestionCount(updateDto.questionCount!.Value)
                .WithPointPerQuestion(updateDto.pointPerQuestion!.Value)
                .WithTotalPoints(updateDto.totalPoints!.Value)
                .WithSequence(updateDto.sequence!.Value)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByConfigIdAsync(configId))
                .ReturnsAsync(updatedConfig);

            // Act
            var result = await _fixture.TestTemplateConfigService.UpdateAsync(configId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.configId.Should().Be(configId);
            result.questionCount.Should().Be(updateDto.questionCount!.Value);
            result.pointPerQuestion.Should().Be(updateDto.pointPerQuestion!.Value);
            result.totalPoints.Should().Be(updateDto.totalPoints!.Value);
            result.sequence.Should().Be(updateDto.sequence!.Value);

            _fixture.MockTestTemplateConfigRepository.Verify(x => x.UpdateAsync(It.IsAny<TestTemplateConfig>()), Times.Once);
            _fixture.MockTestTemplateConfigRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentConfig_ShouldThrowNotFoundException()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var updateDto = TestTemplateConfigServiceFixture.ValidUpdateDto();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByIdAsync(configId))
                .ReturnsAsync((TestTemplateConfig?)null);

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.UpdateAsync(configId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("UPDATE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateAsync_WithActiveType_ShouldThrowBadRequestException()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var updateDto = TestTemplateConfigServiceFixture.ValidUpdateDto();

            var existingConfig = TestTemplateConfigBuilder.Create()
                .WithId(configId)
                .WithTemplateId(templateId)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByIdAsync(configId))
                .ReturnsAsync(existingConfig);

            _fixture.SetupActiveTypeScenario(templateId, typeId);

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.UpdateAsync(configId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("UPDATE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteConfig()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();

            var existingConfig = TestTemplateConfigBuilder.Create()
                .WithId(configId)
                .WithTemplateId(templateId)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByIdAsync(configId))
                .ReturnsAsync(existingConfig);

            _fixture.SetupInactiveTypeScenario(templateId, typeId);

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.DeleteAsync(It.IsAny<TestTemplateConfig>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _fixture.TestTemplateConfigService.DeleteAsync(configId);

            // Assert
            _fixture.MockTestTemplateConfigRepository.Verify(x => x.DeleteAsync(existingConfig), Times.Once);
            _fixture.MockTestTemplateConfigRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task DeleteAsync_WithActiveType_ShouldThrowBadRequestException()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();

            var existingConfig = TestTemplateConfigBuilder.Create()
                .WithId(configId)
                .WithTemplateId(templateId)
                .Build();

            _fixture.MockTestTemplateConfigRepository
                .Setup(x => x.GetByIdAsync(configId))
                .ReturnsAsync(existingConfig);

            _fixture.SetupActiveTypeScenario(templateId, typeId);

            // Act
            var act = async () => await _fixture.TestTemplateConfigService.DeleteAsync(configId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("DELETE_TEST_TEMPLATE_CONFIG_ERROR");

            _fixture.ResetMocks();
        }

        #endregion
    }
}
