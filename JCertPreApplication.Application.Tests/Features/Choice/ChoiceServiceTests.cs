using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Application.Dtos.Choice;
using Xunit;

namespace JCertPreApplication.Application.Tests.Features.Choice
{
    public class ChoiceServiceTests
    {
        private readonly Mock<IChoiceRepository> _choiceRepositoryMock;
        private readonly ChoiceService _choiceService;

        public ChoiceServiceTests()
        {
            _choiceRepositoryMock = new Mock<IChoiceRepository>();
            _choiceService = new ChoiceService(_choiceRepositoryMock.Object);
        }

        [Fact]
        public async Task GetByQuestionIdAsync_WhenChoicesExist_ReturnsMappedDtos()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var choices = new List<JCertPreApplication.Domain.Entities.Choice>
            {
                new JCertPreApplication.Domain.Entities.Choice { choiceId = Guid.NewGuid(), questionId = questionId, choiceText = "A", isCorrect = true },
                new JCertPreApplication.Domain.Entities.Choice { choiceId = Guid.NewGuid(), questionId = questionId, choiceText = "B", isCorrect = false }
            };
            _choiceRepositoryMock.Setup(r => r.GetByQuestionIdAsync(questionId))
                                   .ReturnsAsync(choices);

            // Act
            var result = await _choiceService.GetByQuestionIdAsync(questionId);

            // Assert
            Assert.Equal(choices.Count, result.Count());
            Assert.All(result, dto => Assert.Equal(questionId, dto.QuestionId));
        }

        [Fact]
        public async Task CreateAsync_ValidInput_ReturnsCreatedDto()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var createDto = new ChoiceCreateDto 
            { 
                QuestionId = questionId, 
                Content = "Test Choice", 
                IsCorrect = true 
            };

            var createdChoice = new JCertPreApplication.Domain.Entities.Choice
            {
                choiceId = Guid.NewGuid(),
                questionId = questionId,
                choiceText = createDto.Content,
                isCorrect = createDto.IsCorrect
            };

            _choiceRepositoryMock.Setup(r => r.AddAsync(questionId, It.IsAny<JCertPreApplication.Domain.Entities.Choice>()))
                .ReturnsAsync(createdChoice);

            // Act
            var result = await _choiceService.CreateAsync(questionId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Content, result.Content);
            Assert.Equal(createDto.IsCorrect, result.IsCorrect);
            Assert.Equal(questionId, result.QuestionId);
        }

        [Fact]
        public async Task UpdateListAsync_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var updateDtos = new List<ChoiceUpdateDto>
            {
                new ChoiceUpdateDto { Content = "Updated A", IsCorrect = true },
                new ChoiceUpdateDto { Content = "Updated B", IsCorrect = false }
            };

            // Act
            await _choiceService.UpdateListAsync(questionId, updateDtos);

            // Assert
            _choiceRepositoryMock.Verify(r => r.UpdateListAsync(
                questionId,
                It.Is<IEnumerable<JCertPreApplication.Domain.Entities.Choice>>(choices => 
                    choices.Count() == updateDtos.Count &&
                    choices.All(c => updateDtos.Any(dto => 
                        dto.Content == c.choiceText && 
                        dto.IsCorrect == c.isCorrect)))), 
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDelete()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var choiceId = Guid.NewGuid();

            // Act
            await _choiceService.DeleteAsync(questionId, choiceId);

            // Assert
            _choiceRepositoryMock.Verify(r => r.DeleteAsync(questionId, choiceId), Times.Once);
        }
    }
} 