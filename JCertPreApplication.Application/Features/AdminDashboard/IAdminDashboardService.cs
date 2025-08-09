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
    }
}
