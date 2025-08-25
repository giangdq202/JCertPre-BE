using FluentAssertions;
using JCertPreApplication.Application.Dtos.TestAttempt;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using System.Linq.Expressions;

namespace JCertPreApplication.UnitTests.Features.TestAttempt;

public class TestAttemptServiceTests : IClassFixture<TestAttemptServiceFixture>
{
    private readonly TestAttemptServiceFixture _fixture;

    public TestAttemptServiceTests(TestAttemptServiceFixture fixture)
    {
        _fixture = fixture;
        _fixture.Reset();
    }

    #region StartTestAttemptAsync Tests

        [Fact]
        public async Task StartTestAttemptAsync_WithValidData_ShouldStartAttempt()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var testId = Guid.NewGuid();
            var lessonId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            
            var dto = new StartTestAttemptDto
            {
                UserId = userId,
                TestId = testId
            };

            var test = new Domain.Entities.Test
            {
                testId = testId,
                title = "Sample Test",
                description = "A sample test",
                testType = TestType.CustomManual,
                courseLevel = CourseLevel.N5,
                lessonId = lessonId,
                durationMinutes = 30,
                maxAttempts = 3,
                status = TestStatus.Open,
                availableFrom = DateTime.UtcNow.AddDays(-1),
                availableTo = DateTime.UtcNow.AddDays(1),
                createdByUserId = Guid.NewGuid()
            };

            var lesson = new Domain.Entities.Lesson
            {
                lessonId = lessonId,
                courseId = courseId,
                title = "Sample Lesson"
            };

            var enrollment = new Domain.Entities.Enrollment
            {
                enrollmentId = Guid.NewGuid(),
                userId = userId,
                courseId = courseId
            };

            var existingAttempts = new List<Domain.Entities.TestAttempt>();

            _fixture.MockTestRepository.Setup(r => r.GetByIdAsync(testId))
                .ReturnsAsync(test);

            _fixture.MockLessonRepository.Setup(r => r.GetByIdAsync(lessonId))
                .ReturnsAsync(lesson);

            _fixture.MockEnrollmentRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Domain.Entities.Enrollment, bool>>>(),
                It.IsAny<string>()))
                .ReturnsAsync(enrollment);

            _fixture.MockTestAttemptRepository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<Domain.Entities.TestAttempt, bool>>>(),
                It.IsAny<string>()))
                .ReturnsAsync(existingAttempts);

            _fixture.MockTestAttemptRepository.Setup(r => r.InsertAsync(It.IsAny<Domain.Entities.TestAttempt>()))
                .Returns(Task.FromResult(new Domain.Entities.TestAttempt()));

            _fixture.MockTestAttemptRepository.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _fixture.MockAutoSubmitController.Setup(c => c.AddAttempt(It.IsAny<Guid>(), It.IsAny<DateTime>()));

            // Act
            var result = await _fixture.Service.StartTestAttemptAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.TestId.Should().Be(testId);
            result.Status.Should().Be(TestAttemptStatus.InProgress);
        }

        #endregion

    #region SubmitTestAttemptAsync Tests

    [Fact]
    public async Task SubmitTestAttemptAsync_WithValidAnswers_ShouldCalculateScore()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var testId = Guid.NewGuid();

        var submitDto = new SubmitTestAttemptDto
        {
            AttemptId = attemptId
        };

        var attempt = TestAttemptBuilder.Create()
            .WithId(attemptId)
            .WithTestId(testId)
            .WithStatus(TestAttemptStatus.InProgress)
            .Build();

        var test = TestBuilder.Create()
            .WithId(testId)
            .Build();

        var testQuestions = new List<TestQuestion>();
        var attemptAnswers = new List<AttemptAnswer>();

        _fixture.MockTestAttemptRepository
            .Setup(x => x.GetByIdAsync(attemptId))
            .ReturnsAsync(attempt);

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);

        _fixture.MockTestQuestionRepository
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(testQuestions);

        _fixture.MockAttemptAnswerRepository
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(attemptAnswers);

        _fixture.MockTestScoreSummaryRepository
            .Setup(x => x.InsertAsync(It.IsAny<TestScoreSummary>()))
            .Returns(Task.FromResult(new TestScoreSummary()));

        _fixture.MockTestScoreSummaryRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _fixture.MockTestAttemptRepository
            .Setup(x => x.UpdateAsync(It.IsAny<JCertPreApplication.Domain.Entities.TestAttempt>()))
            .Returns(Task.FromResult(new JCertPreApplication.Domain.Entities.TestAttempt()));

        _fixture.MockTestAttemptRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _fixture.Service.SubmitTestAttemptAsync(submitDto);

        // Assert
        result.Should().NotBeNull();
        result.AttemptId.Should().Be(attemptId);
        result.Status.Should().Be(TestAttemptStatus.Completed);

        _fixture.MockTestAttemptRepository.Verify(x => x.GetByIdAsync(attemptId), Times.Once);
        _fixture.MockTestAttemptRepository.Verify(x => x.UpdateAsync(It.IsAny<JCertPreApplication.Domain.Entities.TestAttempt>()), Times.AtLeastOnce);
    }

    #endregion

    #region GetAllByUserIdAsync Tests

    [Fact]
    public async Task GetUserTestAttemptsAsync_WithValidUserId_ShouldReturnAttempts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var attempts = new List<JCertPreApplication.Domain.Entities.TestAttempt>
        {
            TestAttemptBuilder.Create().WithUserId(userId).Build(),
            TestAttemptBuilder.Create().WithUserId(userId).Build()
        };

        _fixture.MockTestAttemptRepository
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<JCertPreApplication.Domain.Entities.TestAttempt, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(attempts);

        // Act
        var result = await _fixture.Service.GetAllByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(a => a.UserId == userId).Should().BeTrue();
    }

    #endregion

    #region GetAttemptWithScoreSummaryAsync Tests

    [Fact]
    public async Task GetTestAttemptResultAsync_WithValidId_ShouldReturnResult()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var testId = Guid.NewGuid();

        var attempt = TestAttemptBuilder.Create()
            .WithId(attemptId)
            .WithTestId(testId)
            .WithStatus(TestAttemptStatus.Completed)
            .WithIsPass(true)
            .Build();

        var scoreSummary = new TestScoreSummary
        {
            TestAttemptId = attemptId,
            total_score = 85
        };

        _fixture.MockTestAttemptRepository
            .Setup(x => x.GetByIdAsync(attemptId))
            .ReturnsAsync(attempt);

        _fixture.MockTestScoreSummaryRepository
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TestScoreSummary, bool>>>(),
                It.IsAny<string>()))
            .ReturnsAsync(scoreSummary);

        // Act
        var result = await _fixture.Service.GetAttemptWithScoreSummaryAsync(attemptId);

        // Assert
        result.Attempt.Should().NotBeNull();
        result.Attempt.AttemptId.Should().Be(attemptId);
        result.Attempt.Status.Should().Be(TestAttemptStatus.Completed);
        result.ScoreSummary.Should().NotBeNull();
        result.ScoreSummary!.total_score.Should().Be(85);
    }

    #endregion

    #region AutoSubmitExpiredAttemptsAsync Tests

    [Fact] 
    public async Task AutoSubmitExpiredAttemptsAsync_ShouldSubmitExpiredAttempts()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var testId = Guid.NewGuid();

        // Create an expired attempt (started more than test duration ago)
        var expiredAttempt = TestAttemptBuilder.Create()
            .WithId(attemptId)
            .WithTestId(testId)
            .WithStatus(TestAttemptStatus.InProgress)
            .WithStartTime(DateTime.UtcNow.AddMinutes(-70)) // Started 70 minutes ago
            .WithEndTime(DateTime.UtcNow.AddMinutes(-10))   // Should have ended 10 minutes ago
            .Build();

        var test = TestBuilder.Create()
            .WithId(testId)
            .WithDurationMinutes(60) // 60 minute test
            .Build();

        var testQuestions = new List<TestQuestion>();
        var attemptAnswers = new List<AttemptAnswer>();

        // Setup mocks as they would be called by the background service
        _fixture.MockTestAttemptRepository
            .Setup(x => x.GetByIdAsync(attemptId))
            .ReturnsAsync(expiredAttempt);

        _fixture.MockTestRepository
            .Setup(x => x.GetByIdAsync(testId))
            .ReturnsAsync(test);

        _fixture.MockTestQuestionRepository
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TestQuestion, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(testQuestions);

        _fixture.MockAttemptAnswerRepository
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string>()))
            .ReturnsAsync(attemptAnswers);

        _fixture.MockTestScoreSummaryRepository
            .Setup(x => x.InsertAsync(It.IsAny<TestScoreSummary>()))
            .Returns(Task.FromResult(new TestScoreSummary()));

        _fixture.MockTestScoreSummaryRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _fixture.MockTestAttemptRepository
            .Setup(x => x.UpdateAsync(It.IsAny<JCertPreApplication.Domain.Entities.TestAttempt>()))
            .Returns(Task.FromResult(new JCertPreApplication.Domain.Entities.TestAttempt()));

        _fixture.MockTestAttemptRepository
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act - Simulate the auto-submit behavior
        var submitDto = new SubmitTestAttemptDto { AttemptId = attemptId };
        var result = await _fixture.Service.SubmitTestAttemptAsync(submitDto);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(TestAttemptStatus.Completed);
        result.AttemptId.Should().Be(attemptId);

        // Verify the attempt was properly submitted
        _fixture.MockTestAttemptRepository.Verify(x => x.UpdateAsync(
            It.Is<JCertPreApplication.Domain.Entities.TestAttempt>(a => a.status == TestAttemptStatus.Completed)), 
            Times.AtLeastOnce);
    }

    #endregion

    #region GetTestAttemptAsync Tests

    [Fact]
    public async Task GetTestAttemptAsync_WithValidId_ShouldReturnAttempt()
    {
        // Arrange
        var attemptId = Guid.NewGuid();
        var testId = Guid.NewGuid();

        var attempt = TestAttemptBuilder.Create()
            .WithId(attemptId)
            .WithTestId(testId)
            .WithStatus(TestAttemptStatus.InProgress)
            .Build();

        _fixture.MockTestAttemptRepository
            .Setup(x => x.GetByIdAsync(attemptId))
            .ReturnsAsync(attempt);

        // Act
        var result = await _fixture.Service.GetAttemptWithScoreSummaryAsync(attemptId);

        // Assert
        result.Attempt.Should().NotBeNull();
        result.Attempt.AttemptId.Should().Be(attemptId);
        result.Attempt.TestId.Should().Be(testId);
        result.Attempt.Status.Should().Be(TestAttemptStatus.InProgress);
    }

    #endregion
}