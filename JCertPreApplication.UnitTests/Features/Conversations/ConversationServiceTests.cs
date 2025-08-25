using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;
using Xunit;

namespace JCertPreApplication.UnitTests.Features.Conversations
{
    /// <summary>
    /// Unit tests for ConversationService
    /// Testing conversation management including creation, messaging, instructor assignment, and retrieval
    /// </summary>
    public class ConversationServiceTests
    {
        private readonly ConversationServiceFixture _fixture;
        private readonly ConversationService _conversationService;
        private readonly Mock<IConversationRepository> _mockConversationRepository;
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;

        public ConversationServiceTests()
        {
            _fixture = new ConversationServiceFixture();
            _conversationService = _fixture.ConversationService;
            _mockConversationRepository = _fixture.MockConversationRepository;
            _mockMessageRepository = _fixture.MockMessageRepository;
            _mockUserRepository = _fixture.MockUserRepository;
        }

        #region CreateConversationAsync Tests

        [Fact]
        public async Task CreateConversationAsync_WithValidStudent_ShouldCreateConversationWithRandomAcademicManager()
        {
            // Arrange
            var student = ConversationServiceFixture.CreateStudentUser();
            var academicManagers = ConversationServiceFixture.CreateAcademicManagerList(3);
            var createdConversation = ConversationServiceFixture.CreateConversationWithParticipants(
                participants: new List<User> { student, academicManagers[0] });

            _mockUserRepository.Setup(x => x.GetByIdAsync(student.userId))
                .ReturnsAsync(student);
            _mockUserRepository.Setup(x => x.GetAcademicManagersAsync())
                .ReturnsAsync(academicManagers);
            _mockConversationRepository.Setup(x => x.InsertAsync(It.IsAny<Conversation>()))
                .Returns(Task.CompletedTask);
            _mockConversationRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _conversationService.CreateConversationAsync(student.userId);

            // Assert
            result.Should().NotBeNull();
            result.ConversationName.Should().Be("Liên hệ tư vấn lộ trình học");
            result.Participants.Should().HaveCount(2);
            result.Participants.Should().Contain(p => p.Id == student.userId);
            result.Participants.Should().Contain(p => academicManagers.Any(am => am.userId == p.Id));

            _mockUserRepository.Verify(x => x.GetByIdAsync(student.userId), Times.Once);
            _mockUserRepository.Verify(x => x.GetAcademicManagersAsync(), Times.Once);
            _mockConversationRepository.Verify(x => x.InsertAsync(It.IsAny<Conversation>()), Times.Once);
            _mockConversationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateConversationAsync_WithNonExistentStudent_ShouldThrowNotFoundException()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            _mockUserRepository.Setup(x => x.GetByIdAsync(studentId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.CreateConversationAsync(studentId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("INVALID_STUDENT");
            exception.Message.Should().Be("Student not found or user is not a student.");

            _mockUserRepository.Verify(x => x.GetByIdAsync(studentId), Times.Once);
            _mockUserRepository.Verify(x => x.GetAcademicManagersAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateConversationAsync_WithNonStudentUser_ShouldThrowBadRequestException()
        {
            // Arrange
            var nonStudent = ConversationServiceFixture.CreateUserWithWrongRole(roleName: "ADMIN");

            _mockUserRepository.Setup(x => x.GetByIdAsync(nonStudent.userId))
                .ReturnsAsync(nonStudent);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.CreateConversationAsync(nonStudent.userId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("INVALID_STUDENT");
            exception.Message.Should().Be("Student not found or user is not a student.");

            _mockUserRepository.Verify(x => x.GetByIdAsync(nonStudent.userId), Times.Once);
            _mockUserRepository.Verify(x => x.GetAcademicManagersAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateConversationAsync_WithStudentWithoutRole_ShouldThrowBadRequestException()
        {
            // Arrange
            var userWithoutRole = ConversationServiceFixture.CreateUserWithoutRole();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userWithoutRole.userId))
                .ReturnsAsync(userWithoutRole);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.CreateConversationAsync(userWithoutRole.userId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("INVALID_STUDENT");
            exception.Message.Should().Be("Student not found or user is not a student.");

            _mockUserRepository.Verify(x => x.GetByIdAsync(userWithoutRole.userId), Times.Once);
            _mockUserRepository.Verify(x => x.GetAcademicManagersAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateConversationAsync_WithNoAcademicManagers_ShouldThrowInternalServerError()
        {
            // Arrange
            var student = ConversationServiceFixture.CreateStudentUser();

            _mockUserRepository.Setup(x => x.GetByIdAsync(student.userId))
                .ReturnsAsync(student);
            _mockUserRepository.Setup(x => x.GetAcademicManagersAsync())
                .ReturnsAsync(new List<User>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.CreateConversationAsync(student.userId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("NO_ACADEMIC_MANAGER");
            exception.Message.Should().Be("No Academic Manager available in the system.");

            _mockUserRepository.Verify(x => x.GetByIdAsync(student.userId), Times.Once);
            _mockUserRepository.Verify(x => x.GetAcademicManagersAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateConversationAsync_WithInvalidAcademicManager_ShouldThrowInternalServerError()
        {
            // Arrange
            var student = ConversationServiceFixture.CreateStudentUser();
            var invalidAcademicManager = ConversationServiceFixture.CreateUserWithWrongRole(roleName: "ADMIN");
            var academicManagers = new List<User> { invalidAcademicManager };

            _mockUserRepository.Setup(x => x.GetByIdAsync(student.userId))
                .ReturnsAsync(student);
            _mockUserRepository.Setup(x => x.GetAcademicManagersAsync())
                .ReturnsAsync(academicManagers);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.CreateConversationAsync(student.userId));

            exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            exception.ErrorCode.Should().Be("INVALID_ACADEMIC_MANAGER");
            exception.Message.Should().Be("Selected user is not an Academic Manager.");

            _mockUserRepository.Verify(x => x.GetByIdAsync(student.userId), Times.Once);
            _mockUserRepository.Verify(x => x.GetAcademicManagersAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateConversationAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var student = ConversationServiceFixture.CreateStudentUser();
            var academicManagers = ConversationServiceFixture.CreateAcademicManagerList(1);

            _mockUserRepository.Setup(x => x.GetByIdAsync(student.userId))
                .ReturnsAsync(student);
            _mockUserRepository.Setup(x => x.GetAcademicManagersAsync())
                .ReturnsAsync(academicManagers);
            _mockConversationRepository.Setup(x => x.InsertAsync(It.IsAny<Conversation>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _conversationService.CreateConversationAsync(student.userId));

            exception.Message.Should().Be("Database error");

            _mockConversationRepository.Verify(x => x.InsertAsync(It.IsAny<Conversation>()), Times.Once);
        }

        #endregion

        #region SendMessageAsync Tests

        [Fact]
        public async Task SendMessageAsync_WithValidParticipant_ShouldSendMessage()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var sender = ConversationServiceFixture.CreateStudentUser();
            var academicManager = ConversationServiceFixture.CreateAcademicManagerUser();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(
                conversationId, new List<User> { sender, academicManager });
            var messageRequest = ConversationServiceFixture.ValidMessageRequest(sender.userId);
            var createdMessage = MessageBuilder.Create()
                .WithSenderId(sender.userId)
                .WithConversationId(conversationId)
                .WithContent(messageRequest.Content)
                .Build();

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(sender.userId))
                .ReturnsAsync(sender);
            _mockMessageRepository.Setup(x => x.InsertAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message m) => m);
            _mockMessageRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _conversationService.SendMessageAsync(conversationId, messageRequest);

            // Assert
            result.Should().NotBeNull();
            result.Content.Should().Be(messageRequest.Content);
            result.SenderId.Should().Be(sender.userId);
            result.SenderName.Should().Be(sender.fullName);
            result.ConversationId.Should().Be(conversationId);

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(sender.userId), Times.Once);
            _mockMessageRepository.Verify(x => x.InsertAsync(It.IsAny<Message>()), Times.Once);
            _mockMessageRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_WithEmptyContent_ShouldThrowBadRequestException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var messageRequest = ConversationServiceFixture.EmptyContentMessageRequest(senderId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.SendMessageAsync(conversationId, messageRequest));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("INVALID_MESSAGE_CONTENT");
            exception.Message.Should().Be("Message content cannot be null or empty.");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SendMessageAsync_WithWhitespaceContent_ShouldThrowBadRequestException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var messageRequest = ConversationServiceFixture.WhitespaceContentMessageRequest(senderId);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.SendMessageAsync(conversationId, messageRequest));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("INVALID_MESSAGE_CONTENT");
            exception.Message.Should().Be("Message content cannot be null or empty.");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task SendMessageAsync_WithNonExistentConversation_ShouldThrowNotFoundException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var messageRequest = ConversationServiceFixture.ValidMessageRequest(senderId);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync((Conversation?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.SendMessageAsync(conversationId, messageRequest));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_WithNonExistentSender_ShouldThrowNotFoundException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(conversationId);
            var messageRequest = ConversationServiceFixture.ValidMessageRequest(senderId);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(senderId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.SendMessageAsync(conversationId, messageRequest));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(senderId), Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_WithNonParticipantSender_ShouldThrowForbiddenException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var participant = ConversationServiceFixture.CreateStudentUser();
            var nonParticipant = ConversationServiceFixture.CreateAcademicManagerUser();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(
                conversationId, new List<User> { participant });
            var messageRequest = ConversationServiceFixture.ValidMessageRequest(nonParticipant.userId);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(nonParticipant.userId))
                .ReturnsAsync(nonParticipant);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.SendMessageAsync(conversationId, messageRequest));

            exception.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            exception.ErrorCode.Should().Be("SENDER_NOT_PARTICIPANT");
            exception.Message.Should().Be("Sender is not a participant in this conversation.");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(nonParticipant.userId), Times.Once);
            _mockMessageRepository.Verify(x => x.InsertAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task SendMessageAsync_WithValidContentTrimming_ShouldTrimWhitespace()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var sender = ConversationServiceFixture.CreateStudentUser();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(
                conversationId, new List<User> { sender });
            var messageRequest = ConversationServiceFixture.ContentWithWhitespaceMessageRequest(sender.userId);
            var expectedTrimmedContent = messageRequest.Content.Trim();

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(sender.userId))
                .ReturnsAsync(sender);
            _mockMessageRepository.Setup(x => x.InsertAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message m) => m);
            _mockMessageRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _conversationService.SendMessageAsync(conversationId, messageRequest);

            // Assert
            result.Content.Should().Be(expectedTrimmedContent);
            result.Content.Should().NotStartWith(" ");
            result.Content.Should().NotEndWith(" ");

            _mockMessageRepository.Verify(x => x.InsertAsync(It.Is<Message>(m => m.content == expectedTrimmedContent)), Times.Once);
        }

        [Fact]
        public async Task SendMessageAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var sender = ConversationServiceFixture.CreateStudentUser();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(
                conversationId, new List<User> { sender });
            var messageRequest = ConversationServiceFixture.ValidMessageRequest(sender.userId);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(sender.userId))
                .ReturnsAsync(sender);
            _mockMessageRepository.Setup(x => x.InsertAsync(It.IsAny<Message>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _conversationService.SendMessageAsync(conversationId, messageRequest));

            exception.Message.Should().Be("Database error");

            _mockMessageRepository.Verify(x => x.InsertAsync(It.IsAny<Message>()), Times.Once);
        }

        #endregion

        #region AssignInstructorAsync Tests

        [Fact]
        public async Task AssignInstructorAsync_WithValidInstructor_ShouldAddToParticipants()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var student = ConversationServiceFixture.CreateStudentUser();
            var instructor = ConversationServiceFixture.CreateInstructorUser();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(
                conversationId, new List<User> { student });

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(instructor.userId))
                .ReturnsAsync(instructor);
            _mockConversationRepository.Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _conversationService.AssignInstructorAsync(conversationId, instructor.userId);

            // Assert
            conversation.Participants.Should().Contain(p => p.userId == instructor.userId);

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(instructor.userId), Times.Once);
            _mockConversationRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AssignInstructorAsync_WithNonExistentConversation_ShouldThrowNotFoundException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync((Conversation?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.AssignInstructorAsync(conversationId, instructorId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task AssignInstructorAsync_WithNonExistentInstructor_ShouldThrowNotFoundException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var instructorId = Guid.NewGuid();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(conversationId);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(instructorId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.AssignInstructorAsync(conversationId, instructorId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(instructorId), Times.Once);
        }

        [Fact]
        public async Task AssignInstructorAsync_WithNonInstructorUser_ShouldThrowBadRequestException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var nonInstructor = ConversationServiceFixture.CreateStudentUser();
            var conversation = ConversationServiceFixture.CreateConversationWithParticipants(conversationId);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);
            _mockUserRepository.Setup(x => x.GetByIdAsync(nonInstructor.userId))
                .ReturnsAsync(nonInstructor);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.AssignInstructorAsync(conversationId, nonInstructor.userId));

            exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            exception.ErrorCode.Should().Be("INVALID_INSTRUCTOR");
            exception.Message.Should().Be("User is not an instructor.");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
            _mockUserRepository.Verify(x => x.GetByIdAsync(nonInstructor.userId), Times.Once);
            _mockConversationRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region GetConversationAsync Tests

        [Fact]
        public async Task GetConversationAsync_WithValidId_ShouldReturnConversationWithDetails()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var student = ConversationServiceFixture.CreateStudentUser();
            var academicManager = ConversationServiceFixture.CreateAcademicManagerUser();
            var participants = new List<User> { student, academicManager };
            var messages = new List<Message>
            {
                MessageBuilder.Create()
                    .WithSenderId(student.userId)
                    .WithConversationId(conversationId)
                    .WithContent("Hello from student")
                    .Build(),
                MessageBuilder.Create()
                    .WithSenderId(academicManager.userId)
                    .WithConversationId(conversationId)
                    .WithContent("Hello from academic manager")
                    .Build()
            };
            var conversation = ConversationServiceFixture.CreateConversationWithMessages(
                conversationId, participants, messages);

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync(conversation);

            // Act
            var result = await _conversationService.GetConversationAsync(conversationId);

            // Assert
            result.Should().NotBeNull();
            result.ConversationId.Should().Be(conversationId);
            result.Participants.Should().HaveCount(2);
            result.Messages.Should().HaveCount(2);
            result.Participants.Should().Contain(p => p.Id == student.userId);
            result.Participants.Should().Contain(p => p.Id == academicManager.userId);

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
        }

        [Fact]
        public async Task GetConversationAsync_WithNonExistentId_ShouldThrowNotFoundException()
        {
            // Arrange
            var conversationId = Guid.NewGuid();

            _mockConversationRepository.Setup(x => x.GetByIdWithDetailsAsync(conversationId))
                .ReturnsAsync((Conversation?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.GetConversationAsync(conversationId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockConversationRepository.Verify(x => x.GetByIdWithDetailsAsync(conversationId), Times.Once);
        }

        #endregion

        #region GetConversationsForUserAsync Tests

        [Fact]
        public async Task GetConversationsForUserAsync_WithValidUserId_ShouldReturnUserConversations()
        {
            // Arrange
            var user = ConversationServiceFixture.CreateStudentUser();
            var userConversations = ConversationServiceFixture.CreateUserConversations(user, 3);

            _mockUserRepository.Setup(x => x.GetByIdAsync(user.userId))
                .ReturnsAsync(user);
            _mockConversationRepository.Setup(x => x.GetConversationsForUserAsync(user.userId))
                .ReturnsAsync(userConversations);

            // Act
            var result = await _conversationService.GetConversationsForUserAsync(user.userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.All(c => c.Participants.Any(p => p.Id == user.userId)).Should().BeTrue();

            _mockUserRepository.Verify(x => x.GetByIdAsync(user.userId), Times.Once);
            _mockConversationRepository.Verify(x => x.GetConversationsForUserAsync(user.userId), Times.Once);
        }

        [Fact]
        public async Task GetConversationsForUserAsync_WithNonExistentUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(
                () => _conversationService.GetConversationsForUserAsync(userId));

            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");

            _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
            _mockConversationRepository.Verify(x => x.GetConversationsForUserAsync(It.IsAny<Guid>()), Times.Never);
        }

        #endregion
    }
}
