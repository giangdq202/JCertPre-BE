using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Conversation;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Conversation
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public ConversationService(IConversationRepository conversationRepository, IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<ConversationDto> CreateConversationAsync(Guid studentId)
        {
            var student = await _userRepository.GetByIdAsync(studentId);
            if (student == null || student.Role == null || student.Role.roleName != "STUDENT")
            {
                throw ApiException.BadRequest("INVALID_STUDENT", "Student not found or user is not a student.");
            }

            var academicManagers = await _userRepository.GetAcademicManagersAsync();
            if (!academicManagers.Any())
            {
                throw ApiException.InternalServerError("NO_ACADEMIC_MANAGER", "No Academic Manager available in the system.");
            }

            var random = new Random();
            var academicManager = academicManagers.ElementAt(random.Next(academicManagers.Count()));
            if (academicManager.Role == null || academicManager.Role.roleName != "ACADEMIC_MANAGER")
            {
                throw ApiException.InternalServerError("INVALID_ACADEMIC_MANAGER", "Selected user is not an Academic Manager.");
            }

            var conversation = new Domain.Entities.Conversation
            {
                conversationId = Guid.NewGuid(),
                conversationName = "Liên hệ tư vấn lộ trình học",
                createdAt = DateTime.UtcNow,
                Participants = new List<User> { student, academicManager }
            };

            await _conversationRepository.InsertAsync(conversation);
            await _conversationRepository.SaveChangesAsync();

            return MapToConversationDto(conversation);
        }

        public async Task<MessageDto> SendMessageAsync(Guid conversationId, MessageRequest messageRequest)
        {
            // Validate message content
            if (string.IsNullOrWhiteSpace(messageRequest.Content))
            {
                throw ApiException.BadRequest("INVALID_MESSAGE_CONTENT", "Message content cannot be null or empty.");
            }

            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            if (conversation == null)
            {
                throw ApiException.NotFound("Conversation", conversationId);
            }

            var sender = await _userRepository.GetByIdAsync(messageRequest.senderId);
            if (sender == null)
            {
                throw ApiException.NotFound("User", messageRequest.senderId);
            }

            // Verify sender is a participant in the conversation
            if (!conversation.Participants.Any(p => p.userId == messageRequest.senderId))
            {
                throw ApiException.Forbidden("SENDER_NOT_PARTICIPANT", "Sender is not a participant in this conversation.");
            }

            var message = new Message
            {
                messageId = Guid.NewGuid(),
                content = messageRequest.Content.Trim(),
                senderId = messageRequest.senderId,
                conversationId = conversationId,
                sentAt = DateTime.UtcNow
            };

            await _messageRepository.InsertAsync(message);
            await _messageRepository.SaveChangesAsync();

            return MapToMessageDto(message, sender);
        }

        public async Task AssignInstructorAsync(Guid conversationId, Guid instructorId)
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            if (conversation == null)
            {
                throw ApiException.NotFound("Conversation", conversationId);
            }

            var instructor = await _userRepository.GetByIdAsync(instructorId);
            if (instructor == null)
            {
                throw ApiException.NotFound("User", instructorId);
            }

            if (instructor.Role?.roleName != "INSTRUCTOR")
            {
                throw ApiException.BadRequest("INVALID_INSTRUCTOR", "User is not an instructor.");
            }

            // Add instructor to Participants if not already present
            if (!conversation.Participants.Any(p => p.userId == instructorId))
            {
                conversation.Participants.Add(instructor);
                await _conversationRepository.SaveChangesAsync();
            }
        }
        public async Task<ConversationDto> GetConversationAsync(Guid conversationId)
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            if (conversation == null)
            {
                throw ApiException.NotFound("Conversation", conversationId);
            }

            return MapToConversationDto(conversation);
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsForUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw ApiException.NotFound("User", userId);
            }

            var conversations = await _conversationRepository.GetConversationsForUserAsync(userId);
            return conversations.Select(MapToConversationDto);
        }

        #region Private Mapping Methods

        private static ConversationDto MapToConversationDto(Domain.Entities.Conversation conversation)
        {
            return new ConversationDto
            {
                ConversationId = conversation.conversationId,
                ConversationName = conversation.conversationName,
                CreatedAt = conversation.createdAt,
                Participants = conversation.Participants?.Select(MapToAppUserDto).ToList() ?? new List<AppUserDto>(),
                Messages = conversation.Messages?.Select(m => MapToMessageDto(m, conversation.Participants?.FirstOrDefault(p => p.userId == m.senderId))).ToList() ?? new List<MessageDto>()
            };
        }

        private static MessageDto MapToMessageDto(Message message, User? sender)
        {
            return new MessageDto
            {
                MessageId = message.messageId,
                Content = message.content,
                SenderId = message.senderId,
                SenderName = sender?.fullName ?? "Unknown User",
                ConversationId = message.conversationId,
                SentAt = message.sentAt
            };
        }

        private static AppUserDto MapToAppUserDto(User user)
        {
            return new AppUserDto
            {
                Id = user.userId,
                fullName = user.fullName,
                email = user.email,
                phone = user.phone
            };
        }

        #endregion
    }
}
