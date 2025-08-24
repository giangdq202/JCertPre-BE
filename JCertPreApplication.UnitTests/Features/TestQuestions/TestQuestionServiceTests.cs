using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.TestQuestions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
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

namespace JCertPreApplication.UnitTests.Features.TestQuestions;

public class TestQuestionServiceTests
{
    private readonly TestQuestionService _testQuestionService;
    private readonly Mock<ITestQuestionRepository> _mockTestQuestionRepository;
    private readonly Mock<ITestRepository> _mockTestRepository;
    private readonly Mock<ITestAttemptRepository> _mockTestAttemptRepository;
    private readonly Mock<IQuestionRepository> _mockQuestionRepository;
    private readonly Mock<ITestTemplateTypeRepository> _mockTestTemplateTypeRepository;
    private readonly Mock<ITestTemplateRepository> _mockTestTemplateRepository;
    private readonly Mock<ITestTemplateConfigRepository> _mockTestTemplateConfigRepository;

    public TestQuestionServiceTests()
    {
        _mockTestQuestionRepository = new Mock<ITestQuestionRepository>();
        _mockTestRepository = new Mock<ITestRepository>();
        _mockTestAttemptRepository = new Mock<ITestAttemptRepository>();
        _mockQuestionRepository = new Mock<IQuestionRepository>();
        _mockTestTemplateTypeRepository = new Mock<ITestTemplateTypeRepository>();
        _mockTestTemplateRepository = new Mock<ITestTemplateRepository>();
        _mockTestTemplateConfigRepository = new Mock<ITestTemplateConfigRepository>();

        _testQuestionService = new TestQuestionService(
            _mockTestQuestionRepository.Object,
            _mockTestRepository.Object,
            _mockTestAttemptRepository.Object,
            _mockQuestionRepository.Object,
            _mockTestTemplateTypeRepository.Object,
            _mockTestTemplateRepository.Object,
            _mockTestTemplateConfigRepository.Object
        );
    }

    #region AddQuestionsCustomManualAsync Tests

    [Fact]
    public async Task AddQuestionsCustomManualAsync_WithValidData_ShouldAddQuestionsAndAssignNumbers()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var questionId1 = Guid.NewGuid();
        var questionId2 = Guid.NewGuid();
        var pairs = new List<(Guid, Guid)>
        {
            (testId, questionId1),
            (testId, questionId2)
        };

        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);
        var question1 = TestQuestionServiceFixture.CreateActiveQuestion(questionId1);
        var question2 = TestQuestionServiceFixture.CreateActiveQuestion(questionId2);
        var existingTestQuestions = new List<TestQuestion>(); // No existing questions

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(existingTestQuestions);

        _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId1))
            .ReturnsAsync(question1);
        _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId2))
            .ReturnsAsync(question2);

        _mockTestQuestionRepository.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<TestQuestion>>()))
            .Returns(Task.CompletedTask);
        _mockTestQuestionRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _testQuestionService.AddQuestionsCustomManualAsync(pairs);

        // Assert
        _mockTestQuestionRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()), Times.Once);
        _mockQuestionRepository.Verify(x => x.GetByIdAsync(questionId1), Times.Once);
        _mockQuestionRepository.Verify(x => x.GetByIdAsync(questionId2), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<TestQuestion>>(tqs => 
            tqs.Count() == 2 && 
            tqs.First().questionNumber == 1 && 
            tqs.Last().questionNumber == 2)), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddQuestionsCustomManualAsync_WithDuplicateQuestions_ShouldSkipDuplicates()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var questionId1 = Guid.NewGuid();
        var questionId2 = Guid.NewGuid();
        var pairs = new List<(Guid, Guid)>
        {
            (testId, questionId1),
            (testId, questionId2)
        };

        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);
        var question1 = TestQuestionServiceFixture.CreateActiveQuestion(questionId1);
        var question2 = TestQuestionServiceFixture.CreateActiveQuestion(questionId2);
        
        // Question1 already exists in test
        var existingTestQuestions = new List<TestQuestion>
        {
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionId(questionId1).WithQuestionNumber(1).Build()
        };

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(existingTestQuestions);

        _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId1))
            .ReturnsAsync(question1);
        _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId2))
            .ReturnsAsync(question2);

        _mockTestQuestionRepository.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<TestQuestion>>()))
            .Returns(Task.CompletedTask);
        _mockTestQuestionRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _testQuestionService.AddQuestionsCustomManualAsync(pairs);

        // Assert
        _mockTestQuestionRepository.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<TestQuestion>>(tqs => 
            tqs.Count() == 1 && 
            tqs.First().questionId == questionId2 && 
            tqs.First().questionNumber == 2)), Times.Once); // Only question2 added, starting from number 2
    }

    [Fact]
    public async Task AddQuestionsCustomManualAsync_WithInactiveQuestions_ShouldSkipInactive()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var questionId1 = Guid.NewGuid();
        var questionId2 = Guid.NewGuid();
        var pairs = new List<(Guid, Guid)>
        {
            (testId, questionId1),
            (testId, questionId2)
        };

        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);
        var activeQuestion = TestQuestionServiceFixture.CreateActiveQuestion(questionId1);
        var inactiveQuestion = TestQuestionServiceFixture.CreateInactiveQuestion(questionId2);
        var existingTestQuestions = new List<TestQuestion>();

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(existingTestQuestions);

        _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId1))
            .ReturnsAsync(activeQuestion);
        _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId2))
            .ReturnsAsync(inactiveQuestion);

        _mockTestQuestionRepository.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<TestQuestion>>()))
            .Returns(Task.CompletedTask);
        _mockTestQuestionRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _testQuestionService.AddQuestionsCustomManualAsync(pairs);

        // Assert
        _mockTestQuestionRepository.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<TestQuestion>>(tqs => 
            tqs.Count() == 1 && 
            tqs.First().questionId == questionId1)), Times.Once); // Only active question added
    }

    [Fact]
    public async Task AddQuestionsCustomManualAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var pairs = TestQuestionServiceFixture.CreateTestQuestionPairs(testId, 1);
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.AddQuestionsCustomManualAsync(pairs));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("TEST_QUESTION_SERVICE_ERROR");
        exception.Message.Should().Contain("An error occurred while adding questions");
    }

    #endregion

    #region GetQuestionsByTestIdAsync Tests

    [Fact]
    public async Task GetQuestionsByTestIdAsync_WithValidTestId_ShouldReturnOrderedQuestions()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var testQuestions = new List<TestQuestion>
        {
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionNumber(3).Build(),
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionNumber(1).Build(),
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionNumber(2).Build()
        };

        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(testQuestions);

        // Act
        var result = await _testQuestionService.GetQuestionsByTestIdAsync(testId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].QuestionNumber.Should().Be(1);
        result[1].QuestionNumber.Should().Be(2);
        result[2].QuestionNumber.Should().Be(3);

        _mockTestQuestionRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task GetQuestionsByTestIdAsync_WithNoQuestions_ShouldReturnEmptyList()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var emptyTestQuestions = new List<TestQuestion>();

        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(emptyTestQuestions);

        // Act
        var result = await _testQuestionService.GetQuestionsByTestIdAsync(testId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockTestQuestionRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()), Times.Once);
    }

    [Fact]
    public async Task GetQuestionsByTestIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        var testId = Guid.NewGuid();

        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.GetQuestionsByTestIdAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("TEST_QUESTION_SERVICE_ERROR");
        exception.Message.Should().Contain("An error occurred while retrieving test questions");
    }

    #endregion

    #region DeleteTestQuestionAsync Tests

    [Fact]
    public async Task DeleteTestQuestionAsync_WithValidId_ShouldDeleteAndReorderQuestions()
    {
        // Arrange
        var testQuestionId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        var testQuestion = TestQuestionBuilder.Create()
            .WithId(testQuestionId)
            .WithTestId(testId)
            .WithQuestionNumber(2)
            .Build();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);

        _mockTestQuestionRepository.Setup(x => x.GetByIdAsync(testQuestionId))
            .ReturnsAsync(testQuestion);
        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestAttemptRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()))
            .ReturnsAsync(false);
        _mockTestQuestionRepository.Setup(x => x.DeleteAsync(testQuestion))
            .Returns(Task.CompletedTask);
        _mockTestQuestionRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Mock for reorder operation
        var remainingQuestions = new List<TestQuestion>
        {
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionNumber(1).Build(),
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionNumber(3).Build(),
            TestQuestionBuilder.Create().WithTestId(testId).WithQuestionNumber(4).Build()
        };
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(remainingQuestions);
        _mockTestQuestionRepository.Setup(x => x.UpdateAsync(It.IsAny<TestQuestion>()))
            .Returns(Task.CompletedTask);

        // Act
        await _testQuestionService.DeleteTestQuestionAsync(testQuestionId);

        // Assert
        _mockTestQuestionRepository.Verify(x => x.GetByIdAsync(testQuestionId), Times.Once);
        _mockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.Once);
        _mockTestAttemptRepository.Verify(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(testQuestion), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.SaveChangesAsync(), Times.AtLeast(1)); // Called for delete and reorder
    }

    [Fact]
    public async Task DeleteTestQuestionAsync_WithNonCloseTestStatus_ShouldThrowBadRequestException()
    {
        // Arrange
        var testQuestionId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        var testQuestion = TestQuestionBuilder.Create()
            .WithId(testQuestionId)
            .WithTestId(testId)
            .Build();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Open, testId); // Open status

        _mockTestQuestionRepository.Setup(x => x.GetByIdAsync(testQuestionId))
            .ReturnsAsync(testQuestion);
        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.DeleteTestQuestionAsync(testQuestionId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
        exception.Message.Should().Contain("Can only delete question if the test status is closed");

        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTestQuestionAsync_WithActiveAttempts_ShouldThrowBadRequestException()
    {
        // Arrange
        var testQuestionId = Guid.NewGuid();
        var testId = Guid.NewGuid();
        var testQuestion = TestQuestionBuilder.Create()
            .WithId(testQuestionId)
            .WithTestId(testId)
            .Build();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);

        _mockTestQuestionRepository.Setup(x => x.GetByIdAsync(testQuestionId))
            .ReturnsAsync(testQuestion);
        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestAttemptRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()))
            .ReturnsAsync(true); // Active attempt exists

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.DeleteTestQuestionAsync(testQuestionId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
        exception.Message.Should().Contain("Cannot delete question during an active test attempt window");

        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTestQuestionAsync_WithNonExistentTestQuestion_ShouldThrowNotFoundException()
    {
        // Arrange
        var testQuestionId = Guid.NewGuid();

        _mockTestQuestionRepository.Setup(x => x.GetByIdAsync(testQuestionId))
            .ReturnsAsync((TestQuestion?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.DeleteTestQuestionAsync(testQuestionId));

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
        exception.Message.Should().Contain("TestQuestion");
        exception.Message.Should().Contain(testQuestionId.ToString());

        _mockTestRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Never);
    }

    #endregion

    #region DeleteAllTestQuestionsAsync Tests

    [Fact]
    public async Task DeleteAllTestQuestionsAsync_WithValidTestId_ShouldDeleteAllQuestions()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);
        var testQuestions = TestQuestionServiceFixture.CreateTestQuestionsForTest(testId, 3);

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestAttemptRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()))
            .ReturnsAsync(false);
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(testQuestions);
        _mockTestQuestionRepository.Setup(x => x.DeleteAsync(It.IsAny<TestQuestion>()))
            .Returns(Task.CompletedTask);
        _mockTestQuestionRepository.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _testQuestionService.DeleteAllTestQuestionsAsync(testId);

        // Assert
        _mockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.Once);
        _mockTestAttemptRepository.Verify(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Exactly(3));
        _mockTestQuestionRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAllTestQuestionsAsync_WithNonCloseTestStatus_ShouldThrowBadRequestException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Open, testId); // Open status

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.DeleteAllTestQuestionsAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
        exception.Message.Should().Contain("Can only delete questions if the test status is closed");

        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAllTestQuestionsAsync_WithActiveAttempts_ShouldThrowBadRequestException()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestAttemptRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()))
            .ReturnsAsync(true); // Active attempt exists

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _testQuestionService.DeleteAllTestQuestionsAsync(testId));

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("DELETE_NOT_ALLOWED");
        exception.Message.Should().Contain("Cannot delete questions during an active test attempt window");

        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAllTestQuestionsAsync_WithNoQuestions_ShouldReturnWithoutError()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var test = TestQuestionServiceFixture.CreateTestWithStatus(TestStatus.Close, testId);
        var emptyTestQuestions = new List<TestQuestion>();

        _mockTestRepository.Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);
        _mockTestAttemptRepository.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()))
            .ReturnsAsync(false);
        _mockTestQuestionRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()))
            .ReturnsAsync(emptyTestQuestions);

        // Act
        await _testQuestionService.DeleteAllTestQuestionsAsync(testId);

        // Assert - Should complete without error
        _mockTestRepository.Verify(x => x.GetByIdAsync(testId), Times.Once);
        _mockTestAttemptRepository.Verify(x => x.AnyAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>()), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string?>()), Times.Once);
        _mockTestQuestionRepository.Verify(x => x.DeleteAsync(It.IsAny<TestQuestion>()), Times.Never);
        _mockTestQuestionRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion
}
