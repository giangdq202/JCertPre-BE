using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId);
        Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId);
        Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId);
        Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId);
    }
} 