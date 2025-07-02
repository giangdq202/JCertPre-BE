using JCertPreApplication.Application.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.InstructorProfile
{
    public interface IInstructorProfileService
    {
        Task<InstructorProfileDto> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<InstructorProfileDto> GetInstructorProfileAsync(Guid userId);
        Task<InstructorProfileDto> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<bool> DeleteInstructorProfileAsync(Guid userId);
    }
}
