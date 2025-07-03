using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Choices;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Choice;
using Xunit;

namespace JCertPreApplication.Application.Tests.Features.Choice
{
    public class ChoiceServiceTests
    {
        private readonly Mock<IChoiceRepository> _mockChoiceRepository;
        private readonly Mock<IQuestionRepository> _mockQuestionRepository;
        private readonly ChoiceService _choiceService;

        public ChoiceServiceTests()
        {
            _mockChoiceRepository = new Mock<IChoiceRepository>();
            _mockQuestionRepository = new Mock<IQuestionRepository>();
            _choiceService = new ChoiceService(_mockChoiceRepository.Object, _mockQuestionRepository.Object);
        }

        [Fact]
        public async Task GetByQuestionIdAsync_WhenQuestionExists_ReturnsChoices()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var choices = new List<Domain.Entities.Choice>
            {
                new Domain.Entities.Choice { Id = Guid.NewGuid(), QuestionId = questionId, Content = "Choice 1", IsCorrect = true },
                new Domain.Entities.Choice { Id = Guid.NewGuid(), QuestionId = questionId, Content = "Choice 2", IsCorrect = false }
            };

            _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId))
                .ReturnsAsync(new Question { Id = questionId });
            _mockChoiceRepository.Setup(x => x.GetByQuestionIdAsync(questionId))
                .ReturnsAsync(choices);

            // Act
            var result = await _choiceService.GetByQuestionIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByQuestionIdAsync_WhenQuestionDoesNotExist_ThrowsException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId))
                .ReturnsAsync((Question)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _choiceService.GetByQuestionIdAsync(questionId));
        }

        [Fact]
        public async Task GetByQuestionIdAsync_WhenNoChoicesExist_ReturnsEmptyList()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId))
                .ReturnsAsync(new Question { Id = questionId });
            _mockChoiceRepository.Setup(x => x.GetByQuestionIdAsync(questionId))
                .ReturnsAsync(new List<Domain.Entities.Choice>());

            // Act
            var result = await _choiceService.GetByQuestionIdAsync(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ValidChoice_ReturnsCreatedChoice()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var choiceCreateDto = new ChoiceCreateDto
            {
                QuestionId = questionId,
                Content = "Test Choice",
                IsCorrect = true
            };

            _mockQuestionRepository.Setup(x => x.GetByIdAsync(questionId))
                .ReturnsAsync(new Question { Id = questionId });

            _mockChoiceRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Choice>()))
                .ReturnsAsync((Domain.Entities.Choice choice) => choice);

            // Act
            var result = await _choiceService.CreateAsync(choiceCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(choiceCreateDto.Content, result.Content);
            Assert.Equal(choiceCreateDto.IsCorrect, result.IsCorrect);
        }

        [Fact]
        public async Task CreateAsync_InvalidQuestionId_ThrowsException()
        {
            // Arrange
            var choiceCreateDto = new ChoiceCreateDto
            {
                QuestionId = Guid.NewGuid(),
                Content = "Test Choice",
                IsCorrect = true
            };

            _mockQuestionRepository.Setup(x => x.GetByIdAsync(choiceCreateDto.QuestionId))
                .ReturnsAsync((Question)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _choiceService.CreateAsync(choiceCreateDto));
        }

        [Fact]
        public async Task UpdateListAsync_ValidChoices_ReturnsUpdatedChoices()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var choices = new List<ChoiceUpdateDto>
            {
                new ChoiceUpdateDto { Id = Guid.NewGuid(), Content = "Updated Choice 1", IsCorrect = true },
                new ChoiceUpdateDto { Id = Guid.NewGuid(), Content = "Updated Choice 2", IsCorrect = false }
            };

            _mockChoiceRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.Entities.Choice());
            _mockChoiceRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Choice>()))
                .ReturnsAsync((Domain.Entities.Choice choice) => choice);

            // Act
            var result = await _choiceService.UpdateListAsync(choices);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(choices.Count, result.Count);
        }

        [Fact]
        public async Task UpdateListAsync_NonExistentChoice_ThrowsException()
        {
            // Arrange
            var choices = new List<ChoiceUpdateDto>
            {
                new ChoiceUpdateDto { Id = Guid.NewGuid(), Content = "Updated Choice", IsCorrect = true }
            };

            _mockChoiceRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Domain.Entities.Choice)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _choiceService.UpdateListAsync(choices));
        }

        [Fact]
        public async Task UpdateListAsync_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var choices = new List<ChoiceUpdateDto>();

            // Act
            var result = await _choiceService.UpdateListAsync(choices);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteAsync_ExistingChoice_Succeeds()
        {
            // Arrange
            var choiceId = Guid.NewGuid();
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
                .ReturnsAsync(new Domain.Entities.Choice { Id = choiceId });
            _mockChoiceRepository.Setup(x => x.DeleteAsync(It.IsAny<Domain.Entities.Choice>()))
                .Returns(Task.CompletedTask);

            // Act & Assert
            await _choiceService.DeleteAsync(choiceId);
            _mockChoiceRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Choice>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentChoice_ThrowsException()
        {
            // Arrange
            var choiceId = Guid.NewGuid();
            _mockChoiceRepository.Setup(x => x.GetByIdAsync(choiceId))
                .ReturnsAsync((Domain.Entities.Choice)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _choiceService.DeleteAsync(choiceId));
        }
    }
} 