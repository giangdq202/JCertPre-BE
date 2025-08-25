using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Features.Conversation;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures
{
    /// <summary>
    /// Test fixture for ConversationService providing mocked dependencies and helper methods
    /// </summary>
    public class ConversationServiceFixture
    {
        public ConversationService ConversationService { get; }
        public Mock<IConversationRepository> MockConversationRepository { get; }
        public Mock<IMessageRepository> MockMessageRepository { get; }
        public Mock<IUserRepository> MockUserRepository { get; }

        public ConversationServiceFixture()
        {
            MockConversationRepository = new Mock<IConversationRepository>();
            MockMessageRepository = new Mock<IMessageRepository>();
            MockUserRepository = new Mock<IUserRepository>();

            ConversationService = new ConversationService(
                MockConversationRepository.Object,
                MockMessageRepository.Object,
                MockUserRepository.Object
            );
        }

        /// <summary>
        /// Creates a valid MessageRequest for testing
        /// </summary>
        public static MessageRequest ValidMessageRequest(Guid senderId)
        {
            return new MessageRequest
            {
                Content = "Valid message content for testing",
                senderId = senderId
            };
        }

        /// <summary>
        /// Creates a MessageRequest with empty content
        /// </summary>
        public static MessageRequest EmptyContentMessageRequest(Guid senderId)
        {
            return new MessageRequest
            {
                Content = "",
                senderId = senderId
            };
        }

        /// <summary>
        /// Creates a MessageRequest with whitespace content
        /// </summary>
        public static MessageRequest WhitespaceContentMessageRequest(Guid senderId)
        {
            return new MessageRequest
            {
                Content = "   \t\n   ",
                senderId = senderId
            };
        }

        /// <summary>
        /// Creates a MessageRequest with content that needs trimming
        /// </summary>
        public static MessageRequest ContentWithWhitespaceMessageRequest(Guid senderId)
        {
            return new MessageRequest
            {
                Content = "  Valid content with whitespace  ",
                senderId = senderId
            };
        }

        /// <summary>
        /// Creates a student user for testing
        /// </summary>
        public static User CreateStudentUser(Guid? userId = null)
        {
            var studentRole = new Role
            {
                roleId = Guid.NewGuid(),
                roleName = "STUDENT"
            };

            return UserBuilder.Create()
                .WithId(userId ?? Guid.NewGuid())
                .WithRole(studentRole)
                .WithName("Test Student")
                .WithEmail("student@test.com")
                .Build();
        }

        /// <summary>
        /// Creates an academic manager user for testing
        /// </summary>
        public static User CreateAcademicManagerUser(Guid? userId = null)
        {
            var academicManagerRole = new Role
            {
                roleId = Guid.NewGuid(),
                roleName = "ACADEMIC_MANAGER"
            };

            return UserBuilder.Create()
                .WithId(userId ?? Guid.NewGuid())
                .WithRole(academicManagerRole)
                .WithName("Test Academic Manager")
                .WithEmail("academic@test.com")
                .Build();
        }

        /// <summary>
        /// Creates an instructor user for testing
        /// </summary>
        public static User CreateInstructorUser(Guid? userId = null)
        {
            var instructorRole = new Role
            {
                roleId = Guid.NewGuid(),
                roleName = "INSTRUCTOR"
            };

            return UserBuilder.Create()
                .WithId(userId ?? Guid.NewGuid())
                .WithRole(instructorRole)
                .WithName("Test Instructor")
                .WithEmail("instructor@test.com")
                .Build();
        }

        /// <summary>
        /// Creates a user with no role for testing
        /// </summary>
        public static User CreateUserWithoutRole(Guid? userId = null)
        {
            var user = UserBuilder.Create()
                .WithId(userId ?? Guid.NewGuid())
                .WithName("Test User No Role")
                .WithEmail("norole@test.com")
                .Build();
            
            // Set role to null after building
            user.Role = null!;
            
            return user;
        }

        /// <summary>
        /// Creates a user with wrong role for testing
        /// </summary>
        public static User CreateUserWithWrongRole(Guid? userId = null, string roleName = "ADMIN")
        {
            var wrongRole = new Role
            {
                roleId = Guid.NewGuid(),
                roleName = roleName
            };

            return UserBuilder.Create()
                .WithId(userId ?? Guid.NewGuid())
                .WithRole(wrongRole)
                .WithName("Test User Wrong Role")
                .WithEmail("wrongrole@test.com")
                .Build();
        }

        /// <summary>
        /// Creates a list of academic managers for testing
        /// </summary>
        public static List<User> CreateAcademicManagerList(int count)
        {
            var managers = new List<User>();
            for (int i = 0; i < count; i++)
            {
                managers.Add(CreateAcademicManagerUser());
            }
            return managers;
        }

        /// <summary>
        /// Creates a conversation with participants for testing
        /// </summary>
        public static Conversation CreateConversationWithParticipants(
            Guid? conversationId = null, 
            List<User>? participants = null)
        {
            var conversation = ConversationBuilder.Create()
                .WithId(conversationId ?? Guid.NewGuid())
                .Build();

            if (participants != null)
            {
                conversation.Participants = participants;
            }

            return conversation;
        }

        /// <summary>
        /// Creates a conversation with messages for testing
        /// </summary>
        public static Conversation CreateConversationWithMessages(
            Guid? conversationId = null, 
            List<User>? participants = null, 
            List<Message>? messages = null)
        {
            var conversation = CreateConversationWithParticipants(conversationId, participants);
            
            if (messages != null)
            {
                conversation.Messages = messages;
            }

            return conversation;
        }

        /// <summary>
        /// Creates a list of conversations for a user
        /// </summary>
        public static List<Conversation> CreateUserConversations(User user, int count)
        {
            var conversations = new List<Conversation>();
            
            for (int i = 0; i < count; i++)
            {
                var conversation = ConversationBuilder.Create()
                    .WithName($"Conversation {i + 1}")
                    .AddParticipant(user)
                    .Build();
                
                conversations.Add(conversation);
            }
            
            return conversations;
        }
    }
}
