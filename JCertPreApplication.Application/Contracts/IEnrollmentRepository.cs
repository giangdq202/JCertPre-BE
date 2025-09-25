using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId);
        Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId);
        Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId);
        Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId);
        Task<bool> IsUserEnrolledInCourseAsync(Guid userId, Guid courseId);
        Task<bool> IsUserEnrolledInAnyCourseAsync(Guid userId);
        Task<long> GetTotalEnrollmentsCountAsync();
        Task<IEnumerable<MonthlyCount>> GetEnrollmentCountsByMonthAsync(DateTime startDate, DateTime endDate);
        Task<long> CountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<MonthlyRevenue>> GetRevenueByMonthAsync(DateTime startDate, DateTime endDate);
    }
}