using Moq;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Dtos.Message;
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
        public async Task CreateConversationAsync_ValidData_ReturnsConversation()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = new User { Id = studentId, Role = new Role { Name = "Student" } };
            
            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockConversationRepository.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Conversation>()))
                .ReturnsAsync((Domain.Entities.Conversation conv) => conv);

            // Act
            var result = await _conversationService.CreateConversationAsync(studentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(studentId, result.StudentId);
        }

        [Fact]
        public async Task CreateConversationAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.CreateConversationAsync(userId));
        }

        [Fact]
        public async Task CreateConversationAsync_NotStudent_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Role = new Role { Name = "Instructor" } };
            
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.CreateConversationAsync(userId));
        }

        [Fact]
        public async Task CreateConversationAsync_ExistingActiveConversation_ThrowsException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = new User { Id = studentId, Role = new Role { Name = "Student" } };
            var existingConversation = new Domain.Entities.Conversation { StudentId = studentId, InstructorId = Guid.NewGuid() };
            
            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync(student);
            _mockConversationRepository.Setup(x => x.GetActiveConversationForUserAsync(studentId))
                .ReturnsAsync(existingConversation);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.CreateConversationAsync(studentId));
        }

        [Fact]
        public async Task SendMessageAsync_ValidMessage_ReturnsMessage()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messageRequest = new MessageRequest { Content = "Test message" };
            var conversation = new Domain.Entities.Conversation 
            { 
                Id = conversationId,
                StudentId = userId,
                InstructorId = null
            };

            _mockConversationRepository.Setup(x => x.GetByIdAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });
            _mockMessageRepository.Setup(x => x.CreateAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message msg) => msg);

            // Act
            var result = await _conversationService.SendMessageAsync(conversationId, userId, messageRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(messageRequest.Content, result.Content);
        }

        [Fact]
        public async Task SendMessageAsync_ConversationNotFound_ThrowsException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messageRequest = new MessageRequest { Content = "Test message" };

            _mockConversationRepository.Setup(x => x.GetByIdAsync(conversationId))
                .ReturnsAsync((Domain.Entities.Conversation)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.SendMessageAsync(conversationId, userId, messageRequest));
        }

        [Fact]
        public async Task SendMessageAsync_UserNotInConversation_ThrowsException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var messageRequest = new MessageRequest { Content = "Test message" };
            var conversation = new Domain.Entities.Conversation 
            { 
                Id = conversationId,
                StudentId = Guid.NewGuid(),
                InstructorId = Guid.NewGuid()
            };

            _mockConversationRepository.Setup(x => x.GetByIdAsync(conversationId))
                .ReturnsAsync(conversation);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.SendMessageAsync(conversationId, userId, messageRequest));
        }

        [Fact]
        public async Task AssignInstructorAsync_ValidAssignment_ReturnsConversation()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var instructor = new User { Id = instructorId, Role = new Role { Name = "Instructor" } };
            var conversation = new Domain.Entities.Conversation 
            { 
                Id = conversationId,
                StudentId = Guid.NewGuid(),
                InstructorId = null
            };

            _mockConversationRepository.Setup(x => x.GetByIdAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                .ReturnsAsync(instructor);
            _mockConversationRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Conversation>()))
                .ReturnsAsync((Domain.Entities.Conversation conv) => conv);

            // Act
            var result = await _conversationService.AssignInstructorAsync(conversationId, instructorId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(instructorId, result.InstructorId);
        }

        [Fact]
        public async Task AssignInstructorAsync_ConversationNotFound_ThrowsException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();

            _mockConversationRepository.Setup(x => x.GetByIdAsync(conversationId))
                .ReturnsAsync((Domain.Entities.Conversation)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.AssignInstructorAsync(conversationId, instructorId));
        }

        [Fact]
        public async Task AssignInstructorAsync_InstructorNotFound_ThrowsException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var conversation = new Domain.Entities.Conversation { Id = conversationId };

            _mockConversationRepository.Setup(x => x.GetByIdAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.AssignInstructorAsync(conversationId, instructorId));
        }

        [Fact]
        public async Task GetConversationAsync_ExistingConversation_ReturnsConversation()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var conversation = new Domain.Entities.Conversation 
            { 
                Id = conversationId,
                Messages = new List<Message>()
            };

            _mockConversationRepository.Setup(x => x.GetByIdWithMessagesAsync(conversationId))
                .ReturnsAsync(conversation);

            // Act
            var result = await _conversationService.GetConversationAsync(conversationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(conversationId, result.Id);
        }

        [Fact]
        public async Task GetConversationAsync_NonExistentConversation_ThrowsException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            _mockConversationRepository.Setup(x => x.GetByIdWithMessagesAsync(conversationId))
                .ReturnsAsync((Domain.Entities.Conversation)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.GetConversationAsync(conversationId));
        }

        [Fact]
        public async Task GetConversationsForUserAsync_ReturnsConversations()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var conversations = new List<Domain.Entities.Conversation>
            {
                new Domain.Entities.Conversation { Id = Guid.NewGuid(), StudentId = userId },
                new Domain.Entities.Conversation { Id = Guid.NewGuid(), InstructorId = userId }
            };

            _mockConversationRepository.Setup(x => x.GetConversationsForUserAsync(userId))
                .ReturnsAsync(conversations);

            // Act
            var result = await _conversationService.GetConversationsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetConversationsForUserAsync_NoConversations_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockConversationRepository.Setup(x => x.GetConversationsForUserAsync(userId))
                .ReturnsAsync(new List<Domain.Entities.Conversation>());

            // Act
            var result = await _conversationService.GetConversationsForUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetConversationsForUserAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _conversationService.GetConversationsForUserAsync(userId));
        }
    }
} 