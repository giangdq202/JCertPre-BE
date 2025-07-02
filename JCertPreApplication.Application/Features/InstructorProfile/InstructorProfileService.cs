using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Conversation;
using JCertPreApplication.Application.Dtos.Profile;
using JCertPreApplication.Application.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JCertPreApplication.Application.Features.InstructorProfile
{
    public class InstructorProfileService : IInstructorProfileService
    {
        private readonly IInstructorProfileRepository _instructorProfileRepository;
        public InstructorProfileService(IInstructorProfileRepository instructorProfileRepository)
        {
            _instructorProfileRepository = instructorProfileRepository ?? throw new ArgumentNullException(nameof(instructorProfileRepository));
        }
        public async Task<InstructorProfileDto> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            var profile = _instructorProfileRepository.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            if (profile == null) {
                throw new ArgumentNullException(nameof(profile), "Instructor profile creation failed.");
            }
            return profile != null ? MapToInstructorProfileDto(profile) : null;
        }

        public Task<bool> DeleteInstructorProfileAsync(Guid userId)
        {
            return _instructorProfileRepository.DeleteInstructorProfileAsync(userId);
        }

        public async Task<InstructorProfileDto> GetInstructorProfileAsync(Guid userId)
        {
            var profile = _instructorProfileRepository.ReadInstructorProfileAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException($"Instructor profile with userId {userId} not found.");
            }
            return profile != null ? MapToInstructorProfileDto(profile) : null;
        }

        public async Task<InstructorProfileDto> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            var profile = _instructorProfileRepository.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
            return MapToInstructorProfileDto(profile);
        }
        private static InstructorProfileDto MapToInstructorProfileDto(Task<Domain.Entities.InstructorProfile> profile)
        {
            return new InstructorProfileDto
            {
                UserId = profile.Result.userId,
                Introduction = profile.Result.introduction,
                Experience = profile.Result.experience,
                TeachingStyle = profile.Result.teachingStyle
            };
        }

    }
}
