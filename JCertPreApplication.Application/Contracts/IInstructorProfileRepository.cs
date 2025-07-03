using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IInstructorProfileRepository : IGenericRepository<InstructorProfile>
    {
        Task<InstructorProfile?> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<InstructorProfile?> ReadInstructorProfileAsync(Guid userId);
        Task<InstructorProfile?> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<bool> DeleteInstructorProfileAsync(Guid userId);
    }
}
