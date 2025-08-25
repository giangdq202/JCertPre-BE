using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Choice;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.Choices;

public class ChoiceServiceTests
{
    private readonly ChoiceService _choiceService;
    private readonly Mock<IChoiceRepository> _mockChoiceRepository;

    public ChoiceServiceTests()
    {
        _mockChoiceRepository = new Mock<IChoiceRepository>();
        _choiceService = new ChoiceService(_mockChoiceRepository.Object);
    }

    #region GetByQuestionIdAsync Tests

    [Fact]
    public async Task GetByQuestionIdAsync_WithValidQuestionId_ShouldReturnChoices()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var choices = ChoiceServiceFixture.CreateChoicesForQuestion(questionId, 3, 1);

        _mockChoiceRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(choices);

        // Act
        var result = await _choiceService.GetByQuestionIdAsync(questionId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        
        var resultList = result.ToList();
        for (int i = 0; i < choices.Count; i++)
        {
            resultList[i].ChoiceId.Should().Be(choices[i].choiceId);
            resultList[i].QuestionId.Should().Be(choices[i].questionId);
            resultList[i].Content.Should().Be(choices[i].choiceText);
            resultList[i].IsCorrect.Should().Be(choices[i].isCorrect);
        }

        _mockChoiceRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task GetByQuestionIdAsync_WithNoChoices_ShouldReturnEmptyCollection()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var emptyChoices = new List<Choice>();

        _mockChoiceRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(emptyChoices);

        // Act
        var result = await _choiceService.GetByQuestionIdAsync(questionId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockChoiceRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task GetByQuestionIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var questionId = Guid.NewGuid();

        _mockChoiceRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.GetByQuestionIdAsync(questionId));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("CHOICE_SERVICE_ERROR");
        exception.Message.Should().Contain("An error occurred while retrieving choices");
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateChoice()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var createDto = ChoiceServiceFixture.ValidCreateDto("New choice", true);
        var existingChoices = ChoiceServiceFixture.CreateChoicesForQuestion(questionId, 2);
        var createdChoice = ChoiceBuilder.Create()
            .WithQuestionId(questionId)
            .WithText(createDto.Content)
            .WithIsCorrect(createDto.IsCorrect)
            .Build();

        _mockChoiceRepository.SetupSequence(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(existingChoices) // First call - check existing choices
            .ReturnsAsync(existingChoices.Concat(new[] { createdChoice }).ToList()); // Second call - after insert

        _mockChoiceRepository.Setup(x => x.InsertAsync(It.IsAny<Choice>()))
            .ReturnsAsync(createdChoice);

        _mockChoiceRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _choiceService.CreateAsync(questionId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.ChoiceId.Should().Be(createdChoice.choiceId);
        result.QuestionId.Should().Be(questionId);
        result.Content.Should().Be(createDto.Content);
        result.IsCorrect.Should().Be(createDto.IsCorrect);

        _mockChoiceRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()), Times.Exactly(2));
        _mockChoiceRepository.Verify(x => x.InsertAsync(It.IsAny<Choice>()), Times.Once);
        _mockChoiceRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenQuestionHas4Choices_ShouldThrowBadRequestException()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var createDto = ChoiceServiceFixture.ValidCreateDto("New choice", true);
        var existingChoices = ChoiceServiceFixture.CreateChoicesForQuestion(questionId, 4); // Already 4 choices

        _mockChoiceRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(existingChoices);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.CreateAsync(questionId, createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("CHOICE_LIMIT");
        exception.Message.Should().Be("A question can only have 4 choices.");

        _mockChoiceRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()), Times.Once);
        _mockChoiceRepository.Verify(x => x.InsertAsync(It.IsAny<Choice>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithConcurrencyIssue_ShouldRollbackAndThrowException()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var createDto = ChoiceServiceFixture.ValidCreateDto("New choice", true);
        var existingChoices = ChoiceServiceFixture.CreateChoicesForQuestion(questionId, 3);
        var createdChoice = ChoiceBuilder.Create().WithQuestionId(questionId).Build();
        
        // Simulate concurrency: after insert, total becomes > 4 choices
        var choicesAfterConcurrentInsert = ChoiceServiceFixture.CreateChoicesForQuestion(questionId, 5);

        _mockChoiceRepository.SetupSequence(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(existingChoices) // First call - 3 choices
            .ReturnsAsync(choicesAfterConcurrentInsert); // Second call - 5 choices (concurrency issue)

        _mockChoiceRepository.Setup(x => x.InsertAsync(It.IsAny<Choice>()))
            .ReturnsAsync(createdChoice);

        _mockChoiceRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockChoiceRepository.Setup(x => x.DeleteAsync(It.IsAny<Choice>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.CreateAsync(questionId, createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("CHOICE_LIMIT");
        exception.Message.Should().Be("A question can only have 4 choices.");

        // Verify rollback occurred
        _mockChoiceRepository.Verify(x => x.DeleteAsync(It.IsAny<Choice>()), Times.Once);
        _mockChoiceRepository.Verify(x => x.SaveChangesAsync(), Times.Exactly(2)); // Once for insert, once for rollback
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var createDto = ChoiceServiceFixture.ValidCreateDto();

        _mockChoiceRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Choice, bool>>>(), It.IsAny<string?>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.CreateAsync(questionId, createDto));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("CHOICE_CREATE_ERROR");
        exception.Message.Should().Contain("An error occurred while creating choice");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateChoice()
    {
        // Arrange
        var choiceId = Guid.NewGuid();
        var updateDto = ChoiceServiceFixture.ValidUpdateDto("Updated content", true);
        var existingChoice = ChoiceBuilder.Create()
            .WithId(choiceId)
            .WithText("Original content")
            .WithIsCorrect(false)
            .Build();

        _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
            .ReturnsAsync(existingChoice);

        _mockChoiceRepository.Setup(x => x.UpdateAsync(It.IsAny<Choice>()))
            .Returns(Task.CompletedTask);

        _mockChoiceRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _choiceService.UpdateAsync(choiceId, updateDto);

        // Assert
        existingChoice.choiceText.Should().Be(updateDto.Content);
        existingChoice.isCorrect.Should().Be(updateDto.IsCorrect!.Value);

        _mockChoiceRepository.Verify(x => x.GetByIdAsync(choiceId), Times.Once);
        _mockChoiceRepository.Verify(x => x.UpdateAsync(existingChoice), Times.Once);
        _mockChoiceRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentChoice_ShouldThrowNotFoundException()
    {
        // Arrange
        var choiceId = Guid.NewGuid();
        var updateDto = ChoiceServiceFixture.ValidUpdateDto("Updated content");

        _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
            .ReturnsAsync((Choice?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.UpdateAsync(choiceId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("Choice");
        exception.Message.Should().Contain(choiceId.ToString());

        _mockChoiceRepository.Verify(x => x.UpdateAsync(It.IsAny<Choice>()), Times.Never);
        _mockChoiceRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var choiceId = Guid.NewGuid();
        var updateDto = ChoiceServiceFixture.ValidUpdateDto();

        _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.UpdateAsync(choiceId, updateDto));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("CHOICE_UPDATE_ERROR");
        exception.Message.Should().Contain("An error occurred while updating choice");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteChoice()
    {
        // Arrange
        var choiceId = Guid.NewGuid();
        var existingChoice = ChoiceBuilder.Create().WithId(choiceId).Build();

        _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
            .ReturnsAsync(existingChoice);

        _mockChoiceRepository.Setup(x => x.DeleteAsync(It.IsAny<Choice>()))
            .Returns(Task.CompletedTask);

        _mockChoiceRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _choiceService.DeleteAsync(choiceId);

        // Assert
        _mockChoiceRepository.Verify(x => x.GetByIdAsync(choiceId), Times.Once);
        _mockChoiceRepository.Verify(x => x.DeleteAsync(existingChoice), Times.Once);
        _mockChoiceRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var choiceId = Guid.NewGuid();

        _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
            .ReturnsAsync((Choice?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _choiceService.DeleteAsync(choiceId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("Choice");
        exception.Message.Should().Contain(choiceId.ToString());

        _mockChoiceRepository.Verify(x => x.DeleteAsync(It.IsAny<Choice>()), Times.Never);
        _mockChoiceRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion
}
