using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.InstructorProfile
{
    public interface IInstructorProfileService
    {
        Task<Domain.Entities.InstructorProfile> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<Domain.Entities.InstructorProfile> GetInstructorProfileAsync(Guid userId);
        Task<Domain.Entities.InstructorProfile> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<bool> DeleteInstructorProfileAsync(Guid userId);
    }
}
