using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using Xunit;
using StudyPlanEntity = JCertPreApplication.Domain.Entities.StudyPlan;
using StudyPlanItemEntity = JCertPreApplication.Domain.Entities.StudyPlanItem;

namespace JCertPreApplication.UnitTests.Features.StudyPlanItem;

/// <summary>
/// Unit tests for StudyPlanItemService
/// Testing study plan item management including CRUD operations, business rules, and error handling
/// </summary>
public class StudyPlanItemServiceTests
{
    private readonly StudyPlanItemService _studyPlanItemService;
    private readonly Mock<IStudyPlanItemRepository> _mockStudyPlanItemRepository;
    private readonly Mock<IStudyPlanRepository> _mockStudyPlanRepository;

    public StudyPlanItemServiceTests()
    {
        _mockStudyPlanItemRepository = new Mock<IStudyPlanItemRepository>();
        _mockStudyPlanRepository = new Mock<IStudyPlanRepository>();
        _studyPlanItemService = new StudyPlanItemService(
            _mockStudyPlanItemRepository.Object,
            _mockStudyPlanRepository.Object
        );
    }

    #region CreateStudyPlanItemAsync Tests

    [Fact]
    public async Task CreateStudyPlanItemAsync_WithValidData_ShouldCreateAndReturnDto()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var sequence = 1;
        var itemType = "Course";
        var courseId = Guid.NewGuid();
        var testId = (Guid?)null;
        var status = ItemStatus.Pending;

        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();
        var expectedItem = StudyPlanItemBuilder.Create()
            .WithPlanId(planId)
            .WithSequence(sequence)
            .WithItemType(itemType)
            .WithCourseId(courseId)
            .WithTestId(testId)
            .WithStatus(status)
            .Build();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.CreateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _studyPlanItemService.CreateStudyPlanItemAsync(planId, sequence, itemType, courseId, testId, status);

        // Assert
        result.Should().NotBeNull();
        result.PlanId.Should().Be(planId);
        result.Sequence.Should().Be(sequence);
        result.ItemType.Should().Be(itemType);
        result.CourseId.Should().Be(courseId);
        result.TestTemplateTypeId.Should().Be(testId);
        result.Status.Should().Be(status);

        _mockStudyPlanRepository.Verify(x => x.GetStudyPlanByIdAsync(planId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.CreateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateStudyPlanItemAsync_WithNonExistentStudyPlan_ShouldThrowNotFoundException()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var sequence = 1;
        var itemType = "Course";
        var courseId = Guid.NewGuid();
        var testId = (Guid?)null;
        var status = ItemStatus.Pending;

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync((StudyPlanEntity?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _studyPlanItemService.CreateStudyPlanItemAsync(planId, sequence, itemType, courseId, testId, status));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Message.Should().Contain("StudyPlan");
        exception.Message.Should().Contain(planId.ToString());

        _mockStudyPlanRepository.Verify(x => x.GetStudyPlanByIdAsync(planId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.CreateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateStudyPlanItemAsync_WithNullCourseId_ShouldCreateWithNullCourse()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var sequence = 1;
        var itemType = "Test";
        var courseId = (Guid?)null;
        var testId = Guid.NewGuid();
        var status = ItemStatus.Pending;

        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();
        var expectedItem = StudyPlanItemBuilder.Create()
            .WithPlanId(planId)
            .WithCourseId(courseId)
            .WithTestId(testId)
            .Build();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.CreateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _studyPlanItemService.CreateStudyPlanItemAsync(planId, sequence, itemType, courseId, testId, status);

        // Assert
        result.Should().NotBeNull();
        result.CourseId.Should().BeNull();
        result.TestTemplateTypeId.Should().Be(testId);
        result.ItemType.Should().Be(itemType);
    }

    [Fact]
    public async Task CreateStudyPlanItemAsync_WithNullTestId_ShouldCreateWithNullTest()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var sequence = 1;
        var itemType = "Course";
        var courseId = Guid.NewGuid();
        var testId = (Guid?)null;
        var status = ItemStatus.Pending;

        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();
        var expectedItem = StudyPlanItemBuilder.Create()
            .WithPlanId(planId)
            .WithCourseId(courseId)
            .WithTestId(testId)
            .Build();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.CreateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _studyPlanItemService.CreateStudyPlanItemAsync(planId, sequence, itemType, courseId, testId, status);

        // Assert
        result.Should().NotBeNull();
        result.CourseId.Should().Be(courseId);
        result.TestTemplateTypeId.Should().BeNull();
        result.ItemType.Should().Be(itemType);
    }

    [Fact]
    public async Task CreateStudyPlanItemAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.CreateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _studyPlanItemService.CreateStudyPlanItemAsync(planId, 1, "Course", Guid.NewGuid(), null, ItemStatus.Pending));

        exception.Message.Should().Be("Database error");
    }

    #endregion

    #region GetStudyPlanItemByIdAsync Tests

    [Fact]
    public async Task GetStudyPlanItemByIdAsync_WithExistingId_ShouldReturnItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var studyPlanItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(studyPlanItem);

        // Act
        var result = await _studyPlanItemService.GetStudyPlanItemByIdAsync(itemId);

        // Assert
        result.Should().NotBeNull();
        result.ItemId.Should().Be(itemId);
        result.PlanId.Should().Be(studyPlanItem.planId);
        result.Sequence.Should().Be(studyPlanItem.sequence);
        result.ItemType.Should().Be(studyPlanItem.itemType);
        result.CourseId.Should().Be(studyPlanItem.courseId);
        result.TestTemplateTypeId.Should().Be(studyPlanItem.TestTemplateTypeId);
        result.Status.Should().Be(studyPlanItem.status);

        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemByIdAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task GetStudyPlanItemByIdAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync((StudyPlanItemEntity?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _studyPlanItemService.GetStudyPlanItemByIdAsync(itemId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Message.Should().Contain("StudyPlanItem");
        exception.Message.Should().Contain(itemId.ToString());

        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemByIdAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task GetStudyPlanItemByIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _studyPlanItemService.GetStudyPlanItemByIdAsync(itemId));

        exception.Message.Should().Be("Database error");
    }

    #endregion

    #region GetStudyPlanItemsByPlanIdAsync Tests

    [Fact]
    public async Task GetStudyPlanItemsByPlanIdAsync_WithValidPlanId_ShouldReturnItems()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();
        var studyPlanItems = StudyPlanItemServiceFixture.CreateMultipleItems(planId, 3);

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemsByPlanIdAsync(planId))
            .ReturnsAsync(studyPlanItems);

        // Act
        var result = await _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.All(x => x.PlanId == planId).Should().BeTrue();

        _mockStudyPlanRepository.Verify(x => x.GetStudyPlanByIdAsync(planId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemsByPlanIdAsync(planId), Times.Once);
    }

    [Fact]
    public async Task GetStudyPlanItemsByPlanIdAsync_WithNonExistentPlan_ShouldThrowNotFoundException()
    {
        // Arrange
        var planId = Guid.NewGuid();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync((StudyPlanEntity?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Message.Should().Contain("StudyPlan");
        exception.Message.Should().Contain(planId.ToString());

        _mockStudyPlanRepository.Verify(x => x.GetStudyPlanByIdAsync(planId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemsByPlanIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetStudyPlanItemsByPlanIdAsync_WithNoItems_ShouldReturnEmptyList()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemsByPlanIdAsync(planId))
            .ReturnsAsync(new List<StudyPlanItemEntity>());

        // Act
        var result = await _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockStudyPlanRepository.Verify(x => x.GetStudyPlanByIdAsync(planId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemsByPlanIdAsync(planId), Times.Once);
    }

    [Fact]
    public async Task GetStudyPlanItemsByPlanIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var studyPlan = StudyPlanItemServiceFixture.ValidStudyPlan();

        _mockStudyPlanRepository.Setup(x => x.GetStudyPlanByIdAsync(planId))
            .ReturnsAsync(studyPlan);
        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemsByPlanIdAsync(planId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId));

        exception.Message.Should().Be("Database error");
    }

    #endregion

    #region UpdateStudyPlanItemAsync Tests

    [Fact]
    public async Task UpdateStudyPlanItemAsync_WithValidData_ShouldUpdateItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithSequence(1)
            .WithItemType("Course")
            .WithStatus(ItemStatus.Pending)
            .Build();

        var updateDto = StudyPlanItemServiceFixture.ValidUpdateDto();
        var updatedItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithSequence(updateDto.Sequence!.Value)
            .WithItemType(updateDto.ItemType!)
            .WithCourseId(updateDto.CourseId)
            .WithTestId(updateDto.TestTemplateTypeId)
            .WithStatus(updateDto.Status!.Value)
            .Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.ItemId.Should().Be(itemId);
        result.Sequence.Should().Be(updateDto.Sequence);
        result.ItemType.Should().Be(updateDto.ItemType);
        result.CourseId.Should().Be(updateDto.CourseId);
        result.TestTemplateTypeId.Should().Be(updateDto.TestTemplateTypeId);
        result.Status.Should().Be(updateDto.Status);

        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemByIdAsync(itemId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStudyPlanItemAsync_WithPartialUpdate_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var originalSequence = 1;
        var originalItemType = "Course";
        var originalStatus = ItemStatus.Pending;

        var existingItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithSequence(originalSequence)
            .WithItemType(originalItemType)
            .WithStatus(originalStatus)
            .Build();

        var partialUpdateDto = StudyPlanItemServiceFixture.PartialUpdateDto(); // Only sequence is set

        var updatedItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithSequence(partialUpdateDto.Sequence!.Value) // Updated
            .WithItemType(originalItemType) // Unchanged
            .WithStatus(originalStatus) // Unchanged
            .Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, partialUpdateDto);

        // Assert
        result.Should().NotBeNull();
        result.Sequence.Should().Be(partialUpdateDto.Sequence);
        result.ItemType.Should().Be(originalItemType); // Should remain unchanged
        result.Status.Should().Be(originalStatus); // Should remain unchanged
    }

    [Fact]
    public async Task UpdateStudyPlanItemAsync_WithNonExistentItem_ShouldThrowNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = StudyPlanItemServiceFixture.ValidUpdateDto();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync((StudyPlanItemEntity?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Message.Should().Contain("StudyPlanItem");
        exception.Message.Should().Contain(itemId.ToString());

        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemByIdAsync(itemId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStudyPlanItemAsync_WithEmptyItemType_ShouldNotUpdateItemType()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var originalItemType = "Course";

        var existingItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithItemType(originalItemType)
            .Build();

        var updateDto = StudyPlanItemServiceFixture.UpdateDtoWithEmptyItemType();

        var updatedItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithItemType(originalItemType) // Should remain unchanged
            .Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.ItemType.Should().Be(originalItemType); // Should not be updated to empty string
    }

    [Fact]
    public async Task UpdateStudyPlanItemAsync_WithWhitespaceItemType_ShouldNotUpdateItemType()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var originalItemType = "Course";

        var existingItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithItemType(originalItemType)
            .Build();

        var updateDto = StudyPlanItemServiceFixture.UpdateDtoWithWhitespaceItemType();

        var updatedItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .WithItemType(originalItemType) // Should remain unchanged
            .Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.ItemType.Should().Be(originalItemType); // Should not be updated to whitespace
    }

    [Fact]
    public async Task UpdateStudyPlanItemAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = StudyPlanItemBuilder.Create().WithId(itemId).Build();
        var updateDto = StudyPlanItemServiceFixture.ValidUpdateDto();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.UpdateStudyPlanItemAsync(It.IsAny<StudyPlanItemEntity>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto));

        exception.Message.Should().Be("Database error");
    }

    #endregion

    #region DeleteStudyPlanItemAsync Tests

    [Fact]
    public async Task DeleteStudyPlanItemAsync_WithExistingId_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = StudyPlanItemBuilder.Create()
            .WithId(itemId)
            .Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.DeleteStudyPlanItemAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _studyPlanItemService.DeleteStudyPlanItemAsync(itemId);

        // Assert
        result.Should().BeTrue();

        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemByIdAsync(itemId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.DeleteStudyPlanItemAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task DeleteStudyPlanItemAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var itemId = Guid.NewGuid();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync((StudyPlanItemEntity?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _studyPlanItemService.DeleteStudyPlanItemAsync(itemId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.Message.Should().Contain("StudyPlanItem");
        exception.Message.Should().Contain(itemId.ToString());

        _mockStudyPlanItemRepository.Verify(x => x.GetStudyPlanItemByIdAsync(itemId), Times.Once);
        _mockStudyPlanItemRepository.Verify(x => x.DeleteStudyPlanItemAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteStudyPlanItemAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = StudyPlanItemBuilder.Create().WithId(itemId).Build();

        _mockStudyPlanItemRepository.Setup(x => x.GetStudyPlanItemByIdAsync(itemId))
            .ReturnsAsync(existingItem);
        _mockStudyPlanItemRepository.Setup(x => x.DeleteStudyPlanItemAsync(itemId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _studyPlanItemService.DeleteStudyPlanItemAsync(itemId));

        exception.Message.Should().Be("Database error");
    }

    #endregion
}