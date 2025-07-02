using JCertPreApplication.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.InstructorProfile
{
    public class InstructorProfileService : IInstructorProfileService
    {
        private readonly IInstructorProfileRepository _instructorProfileRepository;
        public InstructorProfileService(IInstructorProfileRepository instructorProfileRepository)
        {
            _instructorProfileRepository = instructorProfileRepository ?? throw new ArgumentNullException(nameof(instructorProfileRepository));
        }
        public Task<Domain.Entities.InstructorProfile> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            return _instructorProfileRepository.CreateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
        }

        public Task<bool> DeleteInstructorProfileAsync(Guid userId)
        {
            return _instructorProfileRepository.DeleteInstructorProfileAsync(userId);
        }

        public Task<Domain.Entities.InstructorProfile> GetInstructorProfileAsync(Guid userId)
        {
            return _instructorProfileRepository.ReadInstructorProfileAsync(userId);
        }

        public Task<Domain.Entities.InstructorProfile> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle)
        {
            return _instructorProfileRepository.UpdateInstructorProfileAsync(userId, introduction, experience, teachingStyle);
        }
    }
}
