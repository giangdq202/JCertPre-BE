using JCertPreApplication.Application.Dtos.Profile;

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
