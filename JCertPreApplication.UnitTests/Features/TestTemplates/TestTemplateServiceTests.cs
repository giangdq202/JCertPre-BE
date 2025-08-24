using FluentAssertions;
using JCertPreApplication.Application.Dtos.TestTemplate;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.TestTemplates
{
    public class TestTemplateServiceTests : IClassFixture<TestTemplateServiceFixture>
    {
        private readonly TestTemplateServiceFixture _fixture;

        public TestTemplateServiceTests(TestTemplateServiceFixture fixture)
        {
            _fixture = fixture;
        }

        #region GetAllByTypeIdAsync Tests

        [Fact]
        public async Task GetAllByTypeIdAsync_WithValidTypeId_ShouldReturnTemplates()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var templates = TestTemplateServiceFixture.CreateTemplatesForType(typeId, 3);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _fixture.TestTemplateService.GetAllByTypeIdAsync(typeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.All(x => x.TestTemplateTypeId == typeId).Should().BeTrue();
            
            // Verify repository was called with correct filter
            _fixture.MockTestTemplateRepository.Verify(
                x => x.GetAllAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>(), It.IsAny<string>()), 
                Times.Once);

            // Reset for next test
            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetAllByTypeIdAsync_WithNoTemplates_ShouldReturnEmptyList()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var emptyTemplates = new List<TestTemplate>();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(emptyTemplates);

            // Act
            var result = await _fixture.TestTemplateService.GetAllByTypeIdAsync(typeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task GetAllByTypeIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var typeId = Guid.NewGuid();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestTemplate, bool>>>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateService.GetAllByTypeIdAsync(typeId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("GET_TEST_TEMPLATE_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateTemplate()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var createDto = TestTemplateServiceFixture.ValidCreateDto(typeId);
            var testTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsInactive()
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(testTemplateType);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.InsertAsync(It.IsAny<TestTemplate>()))
                .ReturnsAsync(It.IsAny<TestTemplate>());

            _fixture.MockTestTemplateRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _fixture.TestTemplateService.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.TestTemplateTypeId.Should().Be(typeId);
            result.templateName.Should().Be(createDto.templateName);
            result.durationMinutes.Should().Be(createDto.durationMinutes);
            result.totalScore.Should().Be(createDto.totalScore);
            result.toPassPercentage.Should().Be(createDto.toPassPercentage);
            result.sequence.Should().Be(createDto.sequence);

            _fixture.MockTestTemplateRepository.Verify(x => x.InsertAsync(It.IsAny<TestTemplate>()), Times.Once);
            _fixture.MockTestTemplateRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentType_ShouldThrowNotFoundException()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var createDto = TestTemplateServiceFixture.ValidCreateDto(typeId);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync((TestTemplateType?)null);

            // Act
            var act = async () => await _fixture.TestTemplateService.CreateAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Which.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WithActiveType_ShouldThrowBadRequestException()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var createDto = TestTemplateServiceFixture.ValidCreateDto(typeId);
            var activeTestTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsActive() // isActive = true
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(activeTestTemplateType);

            // Act
            var act = async () => await _fixture.TestTemplateService.CreateAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("TYPE_ACTIVE");
            exception.Which.Message.Should().Contain("Cannot create a test template for an active test template type");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var typeId = Guid.NewGuid();
            var createDto = TestTemplateServiceFixture.ValidCreateDto(typeId);
            var testTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsInactive()
                .Build();

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(testTemplateType);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.InsertAsync(It.IsAny<TestTemplate>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var act = async () => await _fixture.TestTemplateService.CreateAsync(createDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.Which.ErrorCode.Should().Be("CREATE_TEST_TEMPLATE_ERROR");

            _fixture.ResetMocks();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var updateDto = TestTemplateServiceFixture.ValidUpdateDto();
            
            var existingTemplate = TestTemplateBuilder.Create()
                .WithId(templateId)
                .WithTypeId(typeId)
                .WithName("Original Name")
                .WithDuration(60)
                .WithScore(100)
                .WithPassPercentage(70m)
                .WithSequence(1)
                .Build();

            var testTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsInactive()
                .Build();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync(existingTemplate);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(testTemplateType);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.UpdateAsync(It.IsAny<TestTemplate>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _fixture.TestTemplateService.UpdateAsync(templateId, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.templateId.Should().Be(templateId);
            result.templateName.Should().Be(updateDto.templateName);
            result.durationMinutes.Should().Be(updateDto.durationMinutes!.Value);
            result.totalScore.Should().Be(updateDto.totalScore!.Value);
            result.toPassPercentage.Should().Be(updateDto.toPassPercentage!.Value);
            result.sequence.Should().Be(updateDto.sequence!.Value);

            _fixture.MockTestTemplateRepository.Verify(x => x.UpdateAsync(It.IsAny<TestTemplate>()), Times.Once);
            _fixture.MockTestTemplateRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentTemplate_ShouldThrowNotFoundException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var updateDto = TestTemplateServiceFixture.ValidUpdateDto();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync((TestTemplate?)null);

            // Act
            var act = async () => await _fixture.TestTemplateService.UpdateAsync(templateId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Which.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task UpdateAsync_WithActiveType_ShouldThrowBadRequestException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            var updateDto = TestTemplateServiceFixture.ValidUpdateDto();
            
            var existingTemplate = TestTemplateBuilder.Create()
                .WithId(templateId)
                .WithTypeId(typeId)
                .Build();

            var activeTestTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsActive() // isActive = true
                .Build();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync(existingTemplate);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(activeTestTemplateType);

            // Act
            var act = async () => await _fixture.TestTemplateService.UpdateAsync(templateId, updateDto);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("TYPE_ACTIVE");
            exception.Which.Message.Should().Contain("Cannot update a test template for an active test template type");

            _fixture.ResetMocks();
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            
            var existingTemplate = TestTemplateBuilder.Create()
                .WithId(templateId)
                .WithTypeId(typeId)
                .Build();

            var testTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsInactive()
                .Build();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync(existingTemplate);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(testTemplateType);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.DeleteAsync(It.IsAny<TestTemplate>()))
                .Returns(Task.CompletedTask);

            _fixture.MockTestTemplateRepository
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            await _fixture.TestTemplateService.DeleteAsync(templateId);

            // Assert
            _fixture.MockTestTemplateRepository.Verify(x => x.DeleteAsync(existingTemplate), Times.Once);
            _fixture.MockTestTemplateRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            _fixture.ResetMocks();
        }

        [Fact]
        public async Task DeleteAsync_WithActiveType_ShouldThrowBadRequestException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var typeId = Guid.NewGuid();
            
            var existingTemplate = TestTemplateBuilder.Create()
                .WithId(templateId)
                .WithTypeId(typeId)
                .Build();

            var activeTestTemplateType = TestTemplateTypeBuilder.Create()
                .WithId(typeId)
                .AsActive() // isActive = true
                .Build();

            _fixture.MockTestTemplateRepository
                .Setup(x => x.GetByIdAsync(templateId))
                .ReturnsAsync(existingTemplate);

            _fixture.MockTestTemplateTypeRepository
                .Setup(x => x.GetByIdAsync(typeId))
                .ReturnsAsync(activeTestTemplateType);

            // Act
            var act = async () => await _fixture.TestTemplateService.DeleteAsync(templateId);

            // Assert
            var exception = await act.Should().ThrowAsync<ApiException>();
            exception.Which.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.Which.ErrorCode.Should().Be("TYPE_ACTIVE");
            exception.Which.Message.Should().Contain("Cannot delete a test template for an active test template type");

            _fixture.ResetMocks();
        }

        #endregion
    }
}
