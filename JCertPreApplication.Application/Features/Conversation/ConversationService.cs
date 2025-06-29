using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
            if (student == null || student.roleId != GetRoleId("Student"))
            {
                throw new ArgumentException("Invalid student.");
            }

            // Lấy danh sách Academic Manager
            var academicManagers = await _userRepository.GetAllAsync(u => u.Role.roleName == "Academic Manager");
            if (!academicManagers.Any())
            {
                throw new InvalidOperationException("No Academic Manager available.");
            }

            // Chọn ngẫu nhiên một Academic Manager
            var random = new Random();
            var academicManager = academicManagers.ElementAt(random.Next(academicManagers.Count()));

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

        public async Task<Message> SendMessageAsync(Guid conversationId, string content)
        {
            var userId = Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            if (conversation == null || !conversation.Participants.Any(p => p.userId == userId))
            {
                throw new UnauthorizedAccessException("Not authorized to send message.");
            }

            var message = new Message
            {
                messageId = Guid.NewGuid(),
                conversationId = conversationId,
                senderId = userId,
                content = content,
                sentAt = DateTime.UtcNow
            };
            await _messageRepository.InsertAsync(message);
            await _messageRepository.SaveChangesAsync();
            return message;
        }

        public async Task AssignInstructorAsync(Guid conversationId, Guid instructorId)
        {
            var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId);
            var instructor = await _userRepository.GetByIdAsync(instructorId);
            var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
            if (conversation == null || instructor == null || conversation.Participants.FirstOrDefault(p => p.userId == currentUserId)?.roleId != GetRoleId("Academic Manager") || instructor.roleId != GetRoleId("Instructor"))
            {
                throw new UnauthorizedAccessException("Not authorized to assign instructor.");
            }

            conversation.Participants.Add(instructor); // Thêm Instructor vào danh sách Participants
            await _conversationRepository.UpdateAsync(conversation);
            await _conversationRepository.SaveChangesAsync();
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
