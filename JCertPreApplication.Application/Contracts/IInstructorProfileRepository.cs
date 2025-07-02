using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface IInstructorProfileRepository : IGenericRepository<InstructorProfile>
    {
        Task<InstructorProfile> CreateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<InstructorProfile> ReadInstructorProfileAsync(Guid userId);
        Task<InstructorProfile> UpdateInstructorProfileAsync(Guid userId, string introduction, string? experience, string? teachingStyle);
        Task<bool> DeleteInstructorProfileAsync(Guid userId);
    }
}
