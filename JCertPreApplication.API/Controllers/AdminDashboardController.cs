using JCertPreApplication.Application.Features.AdminDashboard;
using JCertPreApplication.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles admin dashboard operations including analytics and statistics.
    /// </summary>
    [Route("api/admin-dashboard")]
    [ApiController]
    [Tags("Admin Dashboard")]
    [Produces("application/json")]
    // [Authorize(Roles = "Admin")] // Uncomment this if you want to restrict access to admin only
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService ?? throw new ArgumentNullException(nameof(adminDashboardService));
        }

        /// <summary>
        /// Get total revenue from money deposit transactions.
        /// Returns the total amount of money deposited into the system through payment transactions.
        /// </summary>
        /// <returns>Total revenue information including amount, currency, and transaction count</returns>
        /// <response code="200">Returns the total revenue data</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("revenue/total")]
        [ProducesResponseType(typeof(Application.Dtos.AdminDashboard.TotalRevenueDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var totalRevenue = await _adminDashboardService.GetTotalRevenueAsync();
            return Ok(totalRevenue);
        }

        /// <summary>
        /// Get total number of course enrollments.
        /// Returns the total count of all course enrollments in the system.
        /// </summary>
        /// <returns>Total enrollments information including count and calculation timestamp</returns>
        /// <response code="200">Returns the total enrollments data</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("enrollments/total")]
        [ProducesResponseType(typeof(Application.Dtos.AdminDashboard.TotalEnrollmentsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalEnrollments()
        {
            var totalEnrollments = await _adminDashboardService.GetTotalEnrollmentsAsync();
            return Ok(totalEnrollments);
        }

        /// <summary>
        /// Get course enrollments statistics by month for the last 12 months.
        /// Returns enrollment counts grouped by month in MM/yyyy format.
        /// </summary>
        /// <returns>Enrollments by month data as a dictionary with MM/yyyy keys and count values</returns>
        /// <response code="200">Returns the enrollments by month data</response>
        /// <response code="500">If there was an internal server error</response>
        [HttpGet("enrollments/by-month")]
        [ProducesResponseType(typeof(Application.Dtos.AdminDashboard.EnrollmentsByMonthDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEnrollmentsByMonth()
        {
            var enrollmentsByMonth = await _adminDashboardService.GetEnrollmentsByMonthAsync();
            return Ok(enrollmentsByMonth);
        }
    }
}
