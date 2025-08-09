using JCertPreApplication.Application.Dtos.AdminDashboard;

namespace JCertPreApplication.Application.Features.AdminDashboard
{
    /// <summary>
    /// Interface for Admin Dashboard services
    /// </summary>
    public interface IAdminDashboardService
    {
        /// <summary>
        /// Get total revenue from money deposit transactions
        /// </summary>
        /// <returns>Total revenue information</returns>
        Task<TotalRevenueDto> GetTotalRevenueAsync();

        /// <summary>
        /// Get total number of course enrollments
        /// </summary>
        /// <returns>Total enrollments information</returns>
        Task<TotalEnrollmentsDto> GetTotalEnrollmentsAsync();

        /// <summary>
        /// Get course enrollments statistics by month for the last 12 months
        /// </summary>
        /// <returns>Enrollments by month data</returns>
        Task<EnrollmentsByMonthDto> GetEnrollmentsByMonthAsync();
    }
}
