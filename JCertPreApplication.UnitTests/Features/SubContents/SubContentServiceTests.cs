using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.SubContent;
using JCertPreApplication.Application.Dtos.Utilities;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.SubContents;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.SubContents;

/// <summary>
/// Unit tests for SubContentService
/// Testing subcontent management including CRUD operations, advanced search/filtering, and enum helper methods
/// </summary>
public class SubContentServiceTests
{
    private readonly SubContentService _subContentService;
    private readonly Mock<IGenericRepository<SubContent>> _mockRepository;

    public SubContentServiceTests()
    {
        _mockRepository = new Mock<IGenericRepository<SubContent>>();
        _subContentService = new SubContentService(_mockRepository.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WithNoFilters_ShouldReturnAllSubContents()
    {
        // Arrange
        var subContents = CreateSampleSubContents();
        var expectedPagination = CreatePaginationResult(subContents);

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(null, null, null, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(5);
        result.TotalItemsCount.Should().Be(5);
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            1, 10,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithSearchFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var searchTerm = "Kanji";
        var filteredSubContents = CreateSampleSubContents().Where(x => 
            x.ContentName.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        var expectedPagination = CreatePaginationResult(filteredSubContents);

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(searchTerm, null, null, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            1, 10,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithLevelFilter_ShouldReturnFilteredByLevel()
    {
        // Arrange
        var level = CourseLevel.N5;
        var filteredSubContents = CreateSampleSubContents().Where(x => x.Level == level).ToList();
        var expectedPagination = CreatePaginationResult(filteredSubContents);

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(null, level, null, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            1, 10,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithContentNameFilter_ShouldReturnFilteredByContentName()
    {
        // Arrange
        var contentName = ContentName.Kanji;
        var filteredSubContents = CreateSampleSubContents().Where(x => x.ContentName == contentName).ToList();
        var expectedPagination = CreatePaginationResult(filteredSubContents);

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(null, null, contentName, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            1, 10,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithSubContentNameFilter_ShouldReturnFilteredBySubContentName()
    {
        // Arrange
        var subContentName = SubContentName.Mondai1;
        var filteredSubContents = CreateSampleSubContents().Where(x => x.SubContentName == subContentName).ToList();
        var expectedPagination = CreatePaginationResult(filteredSubContents);

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(null, null, null, subContentName, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            1, 10,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        var searchTerm = "Mondai";
        var level = CourseLevel.N5;
        var contentName = ContentName.Kanji;
        var filteredSubContents = CreateSampleSubContents().Where(x => 
            x.Level == level && 
            x.ContentName == contentName &&
            x.SubContentName.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        var expectedPagination = CreatePaginationResult(filteredSubContents);

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(searchTerm, level, contentName, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            1, 10,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithPagination_ShouldReturnPaginatedResults()
    {
        // Arrange
        var subContents = CreateSampleSubContents();
        var pageIndex = 2;
        var pageSize = 2;
        var expectedPagination = CreatePaginationResult(subContents.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), pageIndex, pageSize);
        expectedPagination.TotalItemsCount = subContents.Count;

        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            pageIndex,
            pageSize,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ReturnsAsync(expectedPagination);

        // Act
        var result = await _subContentService.GetAllAsync(null, null, null, null, pageIndex, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.PageIndex.Should().Be(pageIndex);
        result.PageSize.Should().Be(pageSize);
        result.TotalItemsCount.Should().Be(subContents.Count);
        _mockRepository.Verify(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            pageIndex, pageSize,
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetPaginationAsync(
            It.IsAny<Expression<Func<SubContent, bool>>>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<Func<IQueryable<SubContent>, IOrderedQueryable<SubContent>>>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _subContentService.GetAllAsync(null, null, null, null, 1, 10));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("SUBCONTENT_SERVICE_ERROR");
        exception.Message.Should().Contain("Error getting subcontents");
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateAndReturnSubContent()
    {
        // Arrange
        var createDto = CreateSubContentDtoBuilder.Create()
            .WithSubContentName(SubContentName.Mondai1)
            .WithLevel(CourseLevel.N5)
            .WithContentName(ContentName.Kanji)
            .Build();

        var expectedSubContent = SubContentBuilder.Create()
            .WithSubContentName(createDto.SubContentName)
            .WithLevel(createDto.Level)
            .WithContentName(createDto.ContentName)
            .Build();

        _mockRepository.Setup(x => x.InsertAsync(It.IsAny<SubContent>()))
            .ReturnsAsync(expectedSubContent);
        _mockRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _subContentService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.SubContentName.Should().Be(createDto.SubContentName);
        result.Level.Should().Be(createDto.Level);
        result.ContentName.Should().Be(createDto.ContentName);
        result.SubContentId.Should().NotBe(Guid.Empty);

        _mockRepository.Verify(x => x.InsertAsync(It.IsAny<SubContent>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidEnumCombination_ShouldCreateWithCorrectValues()
    {
        // Arrange
        var createDto = CreateSubContentDtoBuilder.Create()
            .WithSubContentName(SubContentName.Mondai11)
            .WithLevel(CourseLevel.N1)
            .WithContentName(ContentName.Listening)
            .Build();

        var expectedSubContent = SubContentBuilder.Create()
            .WithSubContentName(createDto.SubContentName)
            .WithLevel(createDto.Level)
            .WithContentName(createDto.ContentName)
            .Build();

        _mockRepository.Setup(x => x.InsertAsync(It.IsAny<SubContent>()))
            .ReturnsAsync(expectedSubContent);
        _mockRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _subContentService.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.SubContentName.Should().Be(SubContentName.Mondai11);
        result.Level.Should().Be(CourseLevel.N1);
        result.ContentName.Should().Be(ContentName.Listening);
        result.SubContentId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var createDto = CreateSubContentDtoBuilder.Create().Build();

        _mockRepository.Setup(x => x.InsertAsync(It.IsAny<SubContent>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _subContentService.CreateAsync(createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("SUBCONTENT_SERVICE_ERROR");
        exception.Message.Should().Contain("Error creating subcontent");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateAndReturnSubContent()
    {
        // Arrange
        var subContentId = Guid.NewGuid();
        var existingSubContent = SubContentBuilder.Create()
            .WithId(subContentId)
            .WithSubContentName(SubContentName.Mondai1)
            .WithLevel(CourseLevel.N5)
            .WithContentName(ContentName.Kanji)
            .Build();

        var updateDto = UpdateSubContentDtoBuilder.Create()
            .WithSubContentName(SubContentName.Mondai2)
            .WithLevel(CourseLevel.N4)
            .WithContentName(ContentName.Vocabulary)
            .Build();

        var updatedSubContent = SubContentBuilder.Create()
            .WithId(subContentId)
            .WithSubContentName(updateDto.SubContentName)
            .WithLevel(updateDto.Level)
            .WithContentName(updateDto.ContentName)
            .Build();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync(existingSubContent);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<SubContent>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _subContentService.UpdateAsync(subContentId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.SubContentId.Should().Be(subContentId);
        result.SubContentName.Should().Be(updateDto.SubContentName);
        result.Level.Should().Be(updateDto.Level);
        result.ContentName.Should().Be(updateDto.ContentName);

        _mockRepository.Verify(x => x.GetByIdAsync(subContentId), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<SubContent>()), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var subContentId = Guid.NewGuid();
        var updateDto = UpdateSubContentDtoBuilder.Create().Build();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync((SubContent?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _subContentService.UpdateAsync(subContentId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("SubContent");
        exception.Message.Should().Contain(subContentId.ToString());
    }

    [Fact]
    public async Task UpdateAsync_WithValidEnumValues_ShouldUpdateAllProperties()
    {
        // Arrange
        var subContentId = Guid.NewGuid();
        var existingSubContent = SubContentBuilder.Create()
            .WithId(subContentId)
            .WithSubContentName(SubContentName.Mondai8)
            .WithLevel(CourseLevel.N2)
            .WithContentName(ContentName.Reading)
            .Build();

        var updateDto = UpdateSubContentDtoBuilder.Create()
            .WithSubContentName(SubContentName.Mondai14)
            .WithLevel(CourseLevel.N1)
            .WithContentName(ContentName.Listening)
            .Build();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync(existingSubContent);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<SubContent>()))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _subContentService.UpdateAsync(subContentId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.SubContentName.Should().Be(SubContentName.Mondai14);
        result.Level.Should().Be(CourseLevel.N1);
        result.ContentName.Should().Be(ContentName.Listening);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var subContentId = Guid.NewGuid();
        var existingSubContent = SubContentBuilder.Create().WithId(subContentId).Build();
        var updateDto = UpdateSubContentDtoBuilder.Create().Build();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync(existingSubContent);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<SubContent>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _subContentService.UpdateAsync(subContentId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("SUBCONTENT_SERVICE_ERROR");
        exception.Message.Should().Contain("Error updating subcontent");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldDeleteSubContent()
    {
        // Arrange
        var subContentId = Guid.NewGuid();
        var existingSubContent = SubContentBuilder.Create().WithId(subContentId).Build();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync(existingSubContent);
        _mockRepository.Setup(x => x.DeleteAsync(existingSubContent))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _subContentService.DeleteAsync(subContentId);

        // Assert
        _mockRepository.Verify(x => x.GetByIdAsync(subContentId), Times.Once);
        _mockRepository.Verify(x => x.DeleteAsync(existingSubContent), Times.Once);
        _mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var subContentId = Guid.NewGuid();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync((SubContent?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _subContentService.DeleteAsync(subContentId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("SubContent");
        exception.Message.Should().Contain(subContentId.ToString());
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var subContentId = Guid.NewGuid();
        var existingSubContent = SubContentBuilder.Create().WithId(subContentId).Build();

        _mockRepository.Setup(x => x.GetByIdAsync(subContentId))
            .ReturnsAsync(existingSubContent);
        _mockRepository.Setup(x => x.DeleteAsync(existingSubContent))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _subContentService.DeleteAsync(subContentId));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("SUBCONTENT_SERVICE_ERROR");
        exception.Message.Should().Contain("Error deleting subcontent");
    }

    #endregion

    #region Enum Helper Tests

    [Fact]
    public async Task GetSubContentNameEnumValuesAsync_ShouldReturnAllSubContentNameValues()
    {
        // Act
        var result = await _subContentService.GetSubContentNameEnumValuesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(15); // SubContentName has 15 values (Mondai1 to Mondai15)
        result.Should().Contain(x => x.Name == "Mondai1" && x.Description == "Đọc chữ Hán");
        result.Should().Contain(x => x.Name == "Mondai14" && x.Description == "Phản hồi tức thời");
        result.Should().Contain(x => x.Name == "Mondai15" && x.Description == "Viết đoạn văn ngắn");
    }

    [Fact]
    public async Task GetLevelEnumValuesAsync_ShouldReturnAllCourseLevelValues()
    {
        // Act
        var result = await _subContentService.GetLevelEnumValuesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(5); // CourseLevel has 5 values (N5 to N1)
        result.Should().Contain(x => x.Name == "N5" && x.Description == "N5");
        result.Should().Contain(x => x.Name == "N1" && x.Description == "N1");
    }

    [Fact]
    public async Task GetContentNameEnumValuesAsync_ShouldReturnAllContentNameValues()
    {
        // Act
        var result = await _subContentService.GetContentNameEnumValuesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(6); // ContentName has 6 values
        result.Should().Contain(x => x.Name == "Kanji" && x.Description == "Chữ Hán");
        result.Should().Contain(x => x.Name == "Vocabulary" && x.Description == "Từ Vựng");
        result.Should().Contain(x => x.Name == "Grammar" && x.Description == "Ngữ Pháp");
        result.Should().Contain(x => x.Name == "Reading" && x.Description == "Đọc Hiểu");
        result.Should().Contain(x => x.Name == "Listening" && x.Description == "Nghe Hiểu");
        result.Should().Contain(x => x.Name == "Writing" && x.Description == "Viết");
    }

    #endregion

    #region Helper Methods

    private List<SubContent> CreateSampleSubContents()
    {
        return new List<SubContent>
        {
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai1)
                .WithLevel(CourseLevel.N5)
                .WithContentName(ContentName.Kanji)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai3)
                .WithLevel(CourseLevel.N4)
                .WithContentName(ContentName.Vocabulary)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai5)
                .WithLevel(CourseLevel.N3)
                .WithContentName(ContentName.Grammar)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai8)
                .WithLevel(CourseLevel.N2)
                .WithContentName(ContentName.Reading)
                .Build(),
            SubContentBuilder.Create()
                .WithSubContentName(SubContentName.Mondai11)
                .WithLevel(CourseLevel.N1)
                .WithContentName(ContentName.Listening)
                .Build()
        };
    }

    private Pagination<SubContent> CreatePaginationResult(List<SubContent> items, int pageIndex = 1, int pageSize = 10)
    {
        return new Pagination<SubContent>
        {
            Items = items,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItemsCount = items.Count
        };
    }

    #endregion
}