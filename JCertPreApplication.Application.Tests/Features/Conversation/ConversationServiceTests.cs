using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Exceptions;
using Xunit;

namespace JCertPreApplication.Application.Tests.Features.Conversation
{
    public class ConversationServiceTests
    {
        private readonly Mock<IConversationRepository> _mockConversationRepository;
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly ConversationService _conversationService;

        public ConversationServiceTests()
        {
            _mockConversationRepository = new Mock<IConversationRepository>();
            _mockMessageRepository = new Mock<IMessageRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _conversationService = new ConversationService(
                _mockConversationRepository.Object,
                _mockMessageRepository.Object,
                _mockUserRepository.Object);
        }

        [Fact]
        public async Task CreateConversationAsync_WhenUserExists_ReturnsConversationDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User 
            { 
                userId = userId, 
                roleId = Guid.NewGuid(),
                fullName = "Test User"
            };
            var role = new Role { roleId = user.roleId, roleName = "Student" };
            user.Role = role;

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var conversation = new Domain.Entities.Conversation
            {
                conversationId = Guid.NewGuid(),
                conversationName = "Test Conversation",
                Participants = new List<User> { user }
            };

            _mockConversationRepository.Setup(x => x.GetConversationsForUserAsync(userId))
                .ReturnsAsync(new List<Domain.Entities.Conversation>());

            _mockConversationRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Conversation>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _conversationService.CreateConversationAsync(userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SendMessageAsync_ValidMessage_ReturnsMessageDto()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messageRequest = new MessageRequest { Content = "Test message" };

            var user = new User 
            { 
                userId = userId,
                fullName = "Test User"
            };
            var conversation = new Domain.Entities.Conversation 
            { 
                conversationId = conversationId,
                conversationName = "Test Conversation",
                Participants = new List<User> { user }
            };

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _mockMessageRepository.Setup(x => x.InsertAsync(It.IsAny<Message>()))
                .ReturnsAsync(new Message());

            // Act
            var result = await _conversationService.SendMessageAsync(conversationId, userId, messageRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(messageRequest.Content, result.Content);
        }

        [Fact]
        public async Task GetConversationAsync_ExistingConversation_ReturnsConversationWithMessages()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var conversation = new Domain.Entities.Conversation
            {
                conversationId = conversationId,
                conversationName = "Test Conversation",
                Messages = new List<Message>(),
                Participants = new List<User>()
            };

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);

            // Act
            var result = await _conversationService.GetConversationAsync(conversationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(conversationId, result.ConversationId);
        }

        [Fact]
        public async Task GetConversationsForUserAsync_ReturnsUserConversations()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var conversations = new List<Domain.Entities.Conversation>
            {
                new Domain.Entities.Conversation
                {
                    conversationId = Guid.NewGuid(),
                    conversationName = "Test Conversation 1",
                    Participants = new List<User>()
                },
                new Domain.Entities.Conversation
                {
                    conversationId = Guid.NewGuid(),
                    conversationName = "Test Conversation 2",
                    Participants = new List<User>()
                }
            };

            _mockConversationRepository.Setup(x => x.GetConversationsForUserAsync(userId))
                .ReturnsAsync(conversations);

            // Act
            var result = await _conversationService.GetConversationsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CreateConversationAsync_UserNotFound_ThrowsApiException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => 
                _conversationService.CreateConversationAsync(userId));
        }

        [Fact]
        public async Task SendMessageAsync_ConversationNotFound_ThrowsApiException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messageRequest = new MessageRequest { Content = "Test message" };

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync((Domain.Entities.Conversation?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => 
                _conversationService.SendMessageAsync(conversationId, userId, messageRequest));
        }

        [Fact]
        public async Task GetConversationAsync_NonExistentConversation_ThrowsApiException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync((Domain.Entities.Conversation?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApiException>(() => 
                _conversationService.GetConversationAsync(conversationId));
        }

        [Fact]
        public async Task GetConversationsForUserAsync_NoConversations_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyConversations = new List<Domain.Entities.Conversation>();

            _mockConversationRepository.Setup(x => x.GetConversationsForUserAsync(userId))
                .ReturnsAsync(emptyConversations);

            // Act
            var result = await _conversationService.GetConversationsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 