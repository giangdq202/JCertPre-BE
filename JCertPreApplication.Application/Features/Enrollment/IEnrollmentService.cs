using JCertPreApplication.Application.Dtos.Enrollment;

namespace JCertPreApplication.Application.Features.Enrollment
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDto> EnrollUserAsync(Guid userId, Guid courseId);
        Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId);
        Task<IEnumerable<EnrollmentResponseDto>> GetUserEnrollmentsAsync(Guid userId);
        Task<bool> UnenrollUserAsync(Guid userId, Guid courseId);
    }
} 