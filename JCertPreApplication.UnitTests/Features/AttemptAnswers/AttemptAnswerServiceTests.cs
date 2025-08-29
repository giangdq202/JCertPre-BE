using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.AttemptAnswer;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.AttemptAnswers;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.AttemptAnswers
{
    /// <summary>
    /// Unit tests for AttemptAnswerService
    /// Testing attempt answer management including add/update operations and retrieval
    /// </summary>
    public class AttemptAnswerServiceTests
    {
        private readonly AttemptAnswerServiceFixture _fixture;
        private readonly AttemptAnswerService _attemptAnswerService;
        private readonly Mock<IAttemptAnswerRepository> _mockAttemptAnswerRepository;
        private readonly Mock<ITestAttemptRepository> _mockTestAttemptRepository;
        private readonly Mock<IChoiceRepository> _mockChoiceRepository;
        private readonly Mock<IQuestionRepository> _mockQuestionRepository;

        public AttemptAnswerServiceTests()
        {
            _fixture = new AttemptAnswerServiceFixture();
            _attemptAnswerService = _fixture.AttemptAnswerService;
            _mockAttemptAnswerRepository = _fixture.MockAttemptAnswerRepository;
            _mockTestAttemptRepository = _fixture.MockTestAttemptRepository;
            _mockChoiceRepository = _fixture.MockChoiceRepository;
            _mockQuestionRepository = _fixture.MockQuestionRepository;
        }

        #region AddOrUpdateAnswersAsync Tests - Happy Path

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithNewAnswer_ShouldCreateNewAnswer()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var choice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
            var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(5, dto.QuestionId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync(choice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                .ReturnsAsync(question);
            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((AttemptAnswer?)null); // No existing answer
            _mockAttemptAnswerRepository.Setup(x => x.InsertAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync((AttemptAnswer a) => a);
            _mockAttemptAnswerRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].AttemptId.Should().Be(dto.AttemptId);
            result[0].QuestionId.Should().Be(dto.QuestionId);
            result[0].ChoiceId.Should().Be(dto.ChoiceId);

            _mockAttemptAnswerRepository.Verify(x => x.InsertAsync(It.Is<AttemptAnswer>(
                a => a.attemptId == dto.AttemptId && 
                     a.questionId == dto.QuestionId && 
                     a.choiceId == dto.ChoiceId &&
                     a.isCorrect == true &&
                     a.score == 5)), Times.Once);
            _mockAttemptAnswerRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithExistingAnswer_ShouldUpdateAnswer()
        {
            // Arrange
            var (dto, existingAnswer) = AttemptAnswerServiceFixture.CreateUpdateScenario();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var newChoice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
            var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(10, dto.QuestionId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync(newChoice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                .ReturnsAsync(question);
            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(existingAnswer); // Return existing answer
            _mockAttemptAnswerRepository.Setup(x => x.UpdateAsync(It.IsAny<AttemptAnswer>()))
                .Returns(Task.CompletedTask);
            _mockAttemptAnswerRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].AttemptId.Should().Be(dto.AttemptId);
            result[0].QuestionId.Should().Be(dto.QuestionId);
            result[0].ChoiceId.Should().Be(dto.ChoiceId);

            // Verify existing answer was updated
            existingAnswer.choiceId.Should().Be(dto.ChoiceId);
            existingAnswer.isCorrect.Should().Be(true);
            existingAnswer.score.Should().Be(10);

            _mockAttemptAnswerRepository.Verify(x => x.UpdateAsync(existingAnswer), Times.Once);
            _mockAttemptAnswerRepository.Verify(x => x.InsertAsync(It.IsAny<AttemptAnswer>()), Times.Never);
            _mockAttemptAnswerRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithCorrectChoice_ShouldCalculateCorrectScore()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var correctChoice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
            var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(8, dto.QuestionId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync(correctChoice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                .ReturnsAsync(question);
            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((AttemptAnswer?)null);
            _mockAttemptAnswerRepository.Setup(x => x.InsertAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync((AttemptAnswer a) => a);
            _mockAttemptAnswerRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);

            _mockAttemptAnswerRepository.Verify(x => x.InsertAsync(It.Is<AttemptAnswer>(
                a => a.isCorrect == true && a.score == 8)), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithIncorrectChoice_ShouldCalculateZeroScore()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var incorrectChoice = AttemptAnswerServiceFixture.CreateIncorrectChoice(dto.ChoiceId, dto.QuestionId);
            var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(5, dto.QuestionId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync(incorrectChoice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                .ReturnsAsync(question);
            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((AttemptAnswer?)null);
            _mockAttemptAnswerRepository.Setup(x => x.InsertAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync((AttemptAnswer a) => a);
            _mockAttemptAnswerRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);

            _mockAttemptAnswerRepository.Verify(x => x.InsertAsync(It.Is<AttemptAnswer>(
                a => a.isCorrect == false && a.score == 0)), Times.Once);
        }

        #endregion

        #region AddOrUpdateAnswersAsync Tests - Validation Errors

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithNonExistentAttempt_ShouldThrowNotFoundException()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var userClaimId = Guid.NewGuid(); // Create a new user ID for this test

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync((JCertPreApplication.Domain.Entities.TestAttempt?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(dto.AttemptId), Times.Once);
            _mockChoiceRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithSuspendedAttempt_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var suspendedAttempt = AttemptAnswerServiceFixture.CreateSuspendedAttempt(dto.AttemptId);
            var userClaimId = suspendedAttempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(suspendedAttempt);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("ATTEMPT_SUSPENDED");
            exception.Message.Should().Be("Cannot add or update answer for a suspended test attempt.");

            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(dto.AttemptId), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithCompletedAttempt_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var completedAttempt = AttemptAnswerServiceFixture.CreateCompletedAttempt(dto.AttemptId);
            var userClaimId = completedAttempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(completedAttempt);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("ATTEMPT_COMPLETED");
            exception.Message.Should().Be("Cannot add or update answer for a completed test attempt.");

            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(dto.AttemptId), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithExpiredAttempt_ShouldThrowBadRequestException()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var expiredAttempt = AttemptAnswerServiceFixture.CreateExpiredAttempt(dto.AttemptId);
            var userClaimId = expiredAttempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(expiredAttempt);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("ATTEMPT_OUT_OF_TIME");
            exception.Message.Should().Be("Cannot add or update answer after test end time.");

            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(dto.AttemptId), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithNonExistentChoice_ShouldThrowNotFoundException()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync((Choice?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(dto.AttemptId), Times.Once);
            _mockChoiceRepository.Verify(x => x.GetByIdAsync(dto.ChoiceId), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithNonExistentQuestion_ShouldThrowNotFoundException()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var choice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync(choice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                .ReturnsAsync((JCertPreApplication.Domain.Entities.Question?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(dto.AttemptId), Times.Once);
            _mockChoiceRepository.Verify(x => x.GetByIdAsync(dto.ChoiceId), Times.Once);
            _mockQuestionRepository.Verify(x => x.GetByIdAsync(dto.QuestionId), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var dto = AttemptAnswerServiceFixture.ValidCreateDto();
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(dto.AttemptId);
            var choice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
            var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(5, dto.QuestionId);
            var userClaimId = attempt.userId; // Use the user ID from the attempt

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(dto.AttemptId))
                .ReturnsAsync(attempt);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                .ReturnsAsync(choice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                .ReturnsAsync(question);
            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(new[] { dto }, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("ATTEMPT_ANSWER_ERROR");
            exception.Message.Should().Be("Database error");
        }

        #endregion

        #region AddOrUpdateAnswersAsync Tests - Batch Processing

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithMultipleAnswers_ShouldProcessAllSuccessfully()
        {
            // Arrange
            var dtos = AttemptAnswerServiceFixture.CreateMultipleDtos(3);
            var attemptId = dtos[0].AttemptId;
            var userClaimId = Guid.NewGuid(); // Create a new user ID for this test
            
            // Use same attempt for all DTOs
            foreach (var dto in dtos)
            {
                dto.AttemptId = attemptId;
            }

            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(attemptId);

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(attemptId))
                .ReturnsAsync(attempt);

            foreach (var dto in dtos)
            {
                var choice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
                var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(5, dto.QuestionId);

                _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                    .ReturnsAsync(choice);
                _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                    .ReturnsAsync(question);
            }

            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((AttemptAnswer?)null); // All new answers
            _mockAttemptAnswerRepository.Setup(x => x.InsertAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync((AttemptAnswer a) => a);
            _mockAttemptAnswerRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _attemptAnswerService.AddOrUpdateAnswersAsync(dtos, userClaimId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            _mockAttemptAnswerRepository.Verify(x => x.InsertAsync(It.IsAny<AttemptAnswer>()), Times.Exactly(3));
            _mockAttemptAnswerRepository.Verify(x => x.SaveChangesAsync(), Times.Once); // Called once at the end
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithMixedNewAndExisting_ShouldHandleBothCorrectly()
        {
            // Arrange
            var (dtos, existingAnswers) = AttemptAnswerServiceFixture.CreateMixedBatchScenario();
            var attemptId = dtos[0].AttemptId;
            var userClaimId = Guid.NewGuid(); // Create a new user ID for this test
            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(attemptId);

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(attemptId))
                .ReturnsAsync(attempt);

            foreach (var dto in dtos)
            {
                var choice = AttemptAnswerServiceFixture.CreateCorrectChoice(dto.ChoiceId, dto.QuestionId);
                var question = AttemptAnswerServiceFixture.CreateQuestionWithPoints(5, dto.QuestionId);

                _mockChoiceRepository.Setup(x => x.GetByIdAsync(dto.ChoiceId))
                    .ReturnsAsync(choice);
                _mockQuestionRepository.Setup(x => x.GetByIdAsync(dto.QuestionId))
                    .ReturnsAsync(question);
            }

            // Setup existing answer for second DTO
            _mockAttemptAnswerRepository.SetupSequence(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((AttemptAnswer?)null) // First DTO - new answer
                .ReturnsAsync(existingAnswers[0]); // Second DTO - existing answer

            _mockAttemptAnswerRepository.Setup(x => x.InsertAsync(It.IsAny<AttemptAnswer>()))
                .ReturnsAsync((AttemptAnswer a) => a);
            _mockAttemptAnswerRepository.Setup(x => x.UpdateAsync(It.IsAny<AttemptAnswer>()))
                .Returns(Task.CompletedTask);
            _mockAttemptAnswerRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _attemptAnswerService.AddOrUpdateAnswersAsync(dtos, userClaimId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            _mockAttemptAnswerRepository.Verify(x => x.InsertAsync(It.IsAny<AttemptAnswer>()), Times.Once);
            _mockAttemptAnswerRepository.Verify(x => x.UpdateAsync(It.IsAny<AttemptAnswer>()), Times.Once);
            _mockAttemptAnswerRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateAnswersAsync_WithPartialFailure_ShouldStopAtFirstError()
        {
            // Arrange
            var dtos = AttemptAnswerServiceFixture.CreateMultipleDtos(2);
            var attemptId = dtos[0].AttemptId;
            var userClaimId = Guid.NewGuid(); // Create a new user ID for this test
            
            foreach (var dto in dtos)
            {
                dto.AttemptId = attemptId;
            }

            var attempt = AttemptAnswerServiceFixture.CreateInProgressAttempt(attemptId);

            _mockTestAttemptRepository.Setup(x => x.GetByIdAsync(attemptId))
                .ReturnsAsync(attempt);

            // First DTO - valid choice and question
            var validChoice = AttemptAnswerServiceFixture.CreateCorrectChoice(dtos[0].ChoiceId, dtos[0].QuestionId);
            var validQuestion = AttemptAnswerServiceFixture.CreateQuestionWithPoints(5, dtos[0].QuestionId);
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dtos[0].ChoiceId))
                .ReturnsAsync(validChoice);
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(dtos[0].QuestionId))
                .ReturnsAsync(validQuestion);
            _mockAttemptAnswerRepository.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync((AttemptAnswer?)null); // No existing answer

            // Second DTO - invalid choice (null) - this should cause the failure
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(dtos[1].ChoiceId))
                .ReturnsAsync((Choice?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.AddOrUpdateAnswersAsync(dtos, userClaimId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            // Verify processing stopped at second DTO when choice was not found
            _mockTestAttemptRepository.Verify(x => x.GetByIdAsync(attemptId), Times.Exactly(2)); // Called for each DTO
            _mockChoiceRepository.Verify(x => x.GetByIdAsync(dtos[0].ChoiceId), Times.Once); // First DTO processed
            _mockChoiceRepository.Verify(x => x.GetByIdAsync(dtos[1].ChoiceId), Times.Once); // Second DTO failed here
            _mockAttemptAnswerRepository.Verify(x => x.SaveChangesAsync(), Times.Never); // Should not save due to error
        }

        #endregion

        #region GetAllByAttemptIdAsync Tests

        [Fact]
        public async Task GetAllByAttemptIdAsync_WithExistingAnswers_ShouldReturnAllAnswers()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var answers = AttemptAnswerServiceFixture.CreateAnswersForAttempt(attemptId, 3);

            _mockAttemptAnswerRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(answers);

            // Act
            var result = await _attemptAnswerService.GetAllByAttemptIdAsync(attemptId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.All(dto => dto.AttemptId == attemptId).Should().BeTrue();

            _mockAttemptAnswerRepository.Verify(x => x.GetAllAsync(
                It.Is<Expression<Func<AttemptAnswer, bool>>>(expr => true), // Accept any expression
                It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByAttemptIdAsync_WithNoAnswers_ShouldReturnEmptyList()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var emptyAnswers = new List<AttemptAnswer>();

            _mockAttemptAnswerRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(emptyAnswers);

            // Act
            var result = await _attemptAnswerService.GetAllByAttemptIdAsync(attemptId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockAttemptAnswerRepository.Verify(x => x.GetAllAsync(
                It.Is<Expression<Func<AttemptAnswer, bool>>>(expr => true),
                It.IsAny<string?>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByAttemptIdAsync_WithValidAttemptId_ShouldReturnMappedDtos()
        {
            // Arrange
            var attemptId = Guid.NewGuid();
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();
            var choiceId1 = Guid.NewGuid();
            var choiceId2 = Guid.NewGuid();

            var answers = new List<AttemptAnswer>
            {
                AttemptAnswerBuilder.Create()
                    .WithAttemptId(attemptId)
                    .WithQuestionId(questionId1)
                    .WithChoiceId(choiceId1)
                    .Build(),
                AttemptAnswerBuilder.Create()
                    .WithAttemptId(attemptId)
                    .WithQuestionId(questionId2)
                    .WithChoiceId(choiceId2)
                    .Build()
            };

            _mockAttemptAnswerRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ReturnsAsync(answers);

            // Act
            var result = await _attemptAnswerService.GetAllByAttemptIdAsync(attemptId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            
            result[0].AttemptId.Should().Be(attemptId);
            result[0].QuestionId.Should().Be(questionId1);
            result[0].ChoiceId.Should().Be(choiceId1);
            
            result[1].AttemptId.Should().Be(attemptId);
            result[1].QuestionId.Should().Be(questionId2);
            result[1].ChoiceId.Should().Be(choiceId2);
        }

        [Fact]
        public async Task GetAllByAttemptIdAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var attemptId = Guid.NewGuid();

            _mockAttemptAnswerRepository.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<AttemptAnswer, bool>>>(), It.IsAny<string?>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _attemptAnswerService.GetAllByAttemptIdAsync(attemptId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("ATTEMPT_ANSWER_ERROR");
            exception.Message.Should().Be("Database connection failed");

            _mockAttemptAnswerRepository.Verify(x => x.GetAllAsync(
                It.IsAny<Expression<Func<AttemptAnswer, bool>>>(),
                It.IsAny<string?>()), Times.Once);
        }

        #endregion
    }
}
