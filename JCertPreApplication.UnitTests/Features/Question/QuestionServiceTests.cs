using FluentAssertions;
using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Questions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.Question;

public class QuestionServiceTests : IClassFixture<QuestionServiceFixture>
{
    private readonly QuestionServiceFixture _fixture;

    public QuestionServiceTests(QuestionServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_ValidQuestion_ShouldReturnCreatedQuestion()
    {
        // Arrange
        var subContent = new SubContentBuilder()
            .WithSubContentId(Guid.NewGuid())
            .WithSubContentName(SubContentName.Mondai1)
            .WithContentName(ContentName.Grammar)
            .WithLevel(CourseLevel.N5)
            .Build();

        var createDto = new CreateQuestionDto
        {
            Content = "What is C#?",
            Explanation = "A programming language",
            Points = 1,
            Difficulty = QuestionDifficulty.Easy,
            IsActive = true,
            ContentName = ContentName.Grammar,
            Level = CourseLevel.N5,
            SubContentName = SubContentName.Mondai1
        };

        var createdQuestion = new QuestionBuilder()
            .WithId(Guid.NewGuid())
            .WithContent("What is C#?")
            .WithDifficulty(QuestionDifficulty.Easy)
            .WithSubContent(subContent)
            .Build();

        _fixture.MockSubContentRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<SubContent, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(subContent);

        _fixture.MockQuestionRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>()))
            .ReturnsAsync(false);

        _fixture.MockQuestionRepository
            .Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Question>()))
            .ReturnsAsync(createdQuestion);

        _fixture.MockQuestionRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _fixture.MockQuestionRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(createdQuestion);

        // Act
        var result = await _fixture.Service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Content.Should().Be("What is C#?");
        result.Difficulty.Should().Be(QuestionDifficulty.Easy);
    }

    [Fact]
    public async Task CreateAsync_InvalidSubContent_ShouldThrowException()
    {
        // Arrange
        var createDto = new CreateQuestionDto
        {
            Content = "What is C#?",
            Difficulty = QuestionDifficulty.Easy,
            ContentName = ContentName.Grammar,
            Level = CourseLevel.N5,
            SubContentName = SubContentName.Mondai1
        };

        _fixture.MockSubContentRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<SubContent, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((SubContent?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(
            () => _fixture.Service.CreateAsync(createDto));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingQuestion_ShouldReturnQuestion()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var expectedQuestion = new QuestionBuilder()
            .WithId(questionId)
            .WithContent("What is C#?")
            .Build();

        _fixture.MockQuestionRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(expectedQuestion);

        // Act
        var result = await _fixture.Service.GetByIdAsync(questionId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(questionId);
        result.Content.Should().Be("What is C#?");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingQuestion_ShouldThrowException()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        _fixture.MockQuestionRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Question?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(
            () => _fixture.Service.GetByIdAsync(questionId));
    }

    [Fact]
    public async Task UpdateAsync_ValidQuestion_ShouldReturnUpdatedQuestion()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var existingQuestion = new QuestionBuilder()
            .WithId(questionId)
            .WithContent("Old content")
            .Build();

        var updateDto = new UpdateQuestionDto
        {
            Content = "Updated content"
        };

        _fixture.MockQuestionRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(existingQuestion);

        _fixture.MockQuestionRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>()))
            .ReturnsAsync(false);

        _fixture.MockQuestionRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Question>()))
            .Returns(Task.CompletedTask);

        _fixture.MockQuestionRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _fixture.Service.UpdateAsync(questionId, updateDto);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_NonExistingQuestion_ShouldThrowException()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var updateDto = new UpdateQuestionDto
        {
            Content = "Updated content"
        };

        _fixture.MockQuestionRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.Question?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(
            () => _fixture.Service.UpdateAsync(questionId, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_DuplicateContent_ShouldThrowException()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var existingQuestion = new QuestionBuilder()
            .WithId(questionId)
            .WithContent("Original content")
            .Build();

        var updateDto = new UpdateQuestionDto
        {
            Content = "Duplicate content"
        };

        _fixture.MockQuestionRepository
            .Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(existingQuestion);

        _fixture.MockQuestionRepository
            .Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Domain.Entities.Question, bool>>>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(
            () => _fixture.Service.UpdateAsync(questionId, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ValidQuestion_ShouldDeleteSuccessfully()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var existingQuestion = new QuestionBuilder()
            .WithId(questionId)
            .WithContent("Question to delete")
            .Build();

        _fixture.MockQuestionRepository
            .Setup(x => x.GetByIdAsync(questionId))
            .ReturnsAsync(existingQuestion);

        _fixture.MockQuestionRepository
            .Setup(x => x.DeleteAsync(It.IsAny<Domain.Entities.Question>()))
            .Returns(Task.CompletedTask);

        _fixture.MockQuestionRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _fixture.Service.DeleteAsync(questionId);

        // Assert
        _fixture.MockQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Question>()), Times.Once);
        _fixture.MockQuestionRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingQuestion_ShouldThrowException()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        _fixture.MockQuestionRepository
            .Setup(x => x.GetByIdAsync(questionId))
            .ReturnsAsync((Domain.Entities.Question?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(
            () => _fixture.Service.DeleteAsync(questionId));
    }
}