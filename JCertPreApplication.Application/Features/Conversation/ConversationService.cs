using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Message;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Conversation
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConversationService(IConversationRepository conversationRepository, IMessageRepository messageRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
            _messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        }

        public async Task<Domain.Entities.Conversation> CreateConversationAsync(Guid studentId)
        {
            var student = await _userRepository.GetByIdAsync(studentId);
            if (student == null || student.Role == null || student.Role.roleName != "Student")
            {
                throw new ArgumentException("Invalid student.");
            }

            var academicManagers = await _userRepository.GetAcademicManagersAsync();
            if (!academicManagers.Any())
            {
                throw new InvalidOperationException("No Academic Manager available.");
            }

            var random = new Random();
            var academicManager = academicManagers.ElementAt(random.Next(academicManagers.Count()));
            if (academicManager.Role == null || academicManager.Role.roleName != "Academic Manager")
            {
                throw new ArgumentException("Invalid academic manager role.");
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
            return conversation;
        }

        public async Task<Message> SendMessageAsync(Guid conversationId, MessageRequest messageRequest)
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            if (conversation == null)
            {
                throw new ArgumentException("Conversation not found.");
            }

            var senderId = messageRequest.senderId;
         

            var sender = await _userRepository.GetByIdAsync(senderId);
            if (sender == null)
            {
                throw new ArgumentException("Sender not found.");
            }

            var message = new Message
            {
                messageId = Guid.NewGuid(),
                content = messageRequest.Content,
                senderId = senderId,
                conversationId = conversationId,
                sentAt = DateTime.UtcNow
            };

            await _messageRepository.InsertAsync(message);
            await _messageRepository.SaveChangesAsync();

            return message;
        }

        public async Task AssignInstructorAsync(Guid conversationId, Guid instructorId)
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            if (conversation == null)
            {
                throw new ArgumentException("Conversation not found.");
            }

            var instructor = await _userRepository.GetByIdAsync(instructorId);
            if (instructor == null)
            {
                throw new ArgumentException("Instructor not found.");
            }

            if (instructor.Role?.roleName != "Instructor")
            {
                throw new ArgumentException("User is not an instructor.");
            }

            // Thêm instructor vào Participants nếu chưa có
            if (!conversation.Participants.Any(p => p.userId == instructorId))
            {
                conversation.Participants.Add(instructor);
                await _conversationRepository.SaveChangesAsync();
            }
        }
        public async Task<List<Message>> GetMyMessagesAsync(Guid userId)
        {
            
            var messageIds = await _messageRepository.GetAllAsync(m => m.senderId == userId);
            return messageIds.ToList();
        }
        public async Task<Domain.Entities.Conversation> GetConversationAsync(Guid conversationId)
        {
            return await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
        }

        private Guid GetRoleId(string roleName)
        {
            // Logic giả lập, cần thay bằng truy vấn thực tế
            var role = _userRepository.GetFirstOrDefaultAsync(r => r.Role.roleName == roleName).Result;
            return role?.roleId ?? Guid.Empty;
        }
    }
}
