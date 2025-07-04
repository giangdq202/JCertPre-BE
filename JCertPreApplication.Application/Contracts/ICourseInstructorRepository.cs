using JCertPreApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface ICourseInstructorRepository : IGenericRepository<CourseInstructor>
    {
        Task<CourseInstructor?> GetByIds(Guid courseId, Guid instructorId);
        Task<IEnumerable<CourseInstructor>> GetActiveByCourseId(Guid courseId);
        Task<IEnumerable<CourseInstructor>> GetHistoryByCourseId(Guid courseId);
        Task<IEnumerable<CourseInstructor>> GetActiveByInstructorId(Guid instructorId);
        Task<IEnumerable<CourseInstructor>> GetHistoryByInstructorId(Guid instructorId);
        Task<bool> IsInstructorAssignedToCourse(Guid courseId, Guid instructorId);
    }
} 