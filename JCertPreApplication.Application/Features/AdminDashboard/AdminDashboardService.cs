using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.AdminDashboard;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.AdminDashboard
{
    /// <summary>
    /// Service implementation for Admin Dashboard operations
    /// </summary>
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public AdminDashboardService(
            IPaymentRepository paymentRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _enrollmentRepository = enrollmentRepository ?? throw new ArgumentNullException(nameof(enrollmentRepository));
        }

        /// <summary>
        /// Get total revenue from money deposit transactions
        /// </summary>
        /// <returns>Total revenue information</returns>
        public async Task<TotalRevenueDto> GetTotalRevenueAsync()
        {
            try
            {
                // Get all payments with type = Money (money deposit transactions)
                

                // Filter by successful payments only
                var enrollments = _enrollmentRepository.GetAllAsync();

                // Calculate total amount
                var totalAmount = enrollments.Result.Sum(e => e.price);

                return new TotalRevenueDto
                {
                    TotalAmount = totalAmount,
                    Currency = "VND",
                    TotalTransactions = totalAmount.Scale,
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                // Log the exception (you can inject ILogger here if needed)
                throw ApiException.InternalServerError(
                    "REVENUE_CALCULATION_ERROR",
                    "An error occurred while calculating total revenue. Please try again later."
                );
            }
        }

        /// <summary>
        /// Get total number of course enrollments
        /// </summary>
        /// <returns>Total enrollments information</returns>
        public async Task<TotalEnrollmentsDto> GetTotalEnrollmentsAsync()
        {
            try
            {
                // Get total count of all enrollments
                var totalCount = await _enrollmentRepository.GetTotalEnrollmentsCountAsync();

                return new TotalEnrollmentsDto
                {
                    TotalCount = totalCount,
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                // Log the exception (you can inject ILogger here if needed)
                throw ApiException.InternalServerError(
                    "ENROLLMENTS_COUNT_ERROR",
                    "An error occurred while calculating total enrollments. Please try again later."
                );
            }
        }

        /// <summary>
        /// Get course enrollments statistics by month for the last 12 months
        /// </summary>
        /// <returns>Enrollments by month data</returns>
        public async Task<EnrollmentsByMonthDto> GetEnrollmentsByMonthAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                // Calculate start date (12 months ago from the beginning of current month)
                var startDate = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1).AddMonths(-11), DateTimeKind.Utc);
                // End date is the beginning of next month
                var endDate = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1).AddMonths(1), DateTimeKind.Utc);

                // Get enrollment counts by month from repository
                var monthlyData = await _enrollmentRepository.GetEnrollmentCountsByMonthAsync(startDate, endDate);

                // Create a dictionary to store results
                var result = new Dictionary<string, long>();

                // Initialize all 12 months with 0 count
                var currentMonth = startDate;
                for (int i = 0; i < 12; i++)
                {
                    var monthKey = currentMonth.ToString("MM/yyyy");
                    result[monthKey] = 0;
                    currentMonth = currentMonth.AddMonths(1);
                }

                // Update with actual data from repository
                foreach (var monthData in monthlyData)
                {
                    var monthKey = $"{monthData.Month:D2}/{monthData.Year}";
                    result[monthKey] = monthData.Count;
                }

                return new EnrollmentsByMonthDto
                {
                    Data = result,
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                // Log the exception (you can inject ILogger here if needed)
                throw ApiException.InternalServerError(
                    "ENROLLMENTS_BY_MONTH_ERROR",
                    "An error occurred while calculating enrollments by month. Please try again later."
                );
            }
        }

        /// <summary>
        /// Get current month enrollments count
        /// </summary>
        /// <returns>Current month enrollments information</returns>
        public async Task<CurrentMonthEnrollmentsDto> GetCurrentMonthEnrollmentsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                // Calculate start of current month
                var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
                // Calculate start of next month
                var startOfNextMonth = DateTime.SpecifyKind(startOfMonth.AddMonths(1), DateTimeKind.Utc);

                // Get enrollment count for current month
                var count = await _enrollmentRepository.CountByDateRangeAsync(startOfMonth, startOfNextMonth);

                return new CurrentMonthEnrollmentsDto
                {
                    Count = count,
                    Month = now.ToString("MM/yyyy"),
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError(
                    "CURRENT_MONTH_ENROLLMENTS_ERROR",
                    "An error occurred while calculating current month enrollments. Please try again later."
                );
            }
        }

        /// <summary>
        /// Get current month revenue amount
        /// </summary>
        /// <returns>Current month revenue information</returns>
        public async Task<CurrentMonthRevenueDto> GetCurrentMonthRevenueAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                // Calculate start of current month
                var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc);
                // Calculate start of next month
                var startOfNextMonth = DateTime.SpecifyKind(startOfMonth.AddMonths(1), DateTimeKind.Utc);

                // Get revenue amount for current month
                var totalAmount = await _enrollmentRepository.GetTotalRevenueByDateRangeAsync(startOfMonth, startOfNextMonth);
                return new CurrentMonthRevenueDto
                {
                    TotalAmount = totalAmount,
                    Currency = "VND",
                    Month = now.ToString("MM/yyyy"),
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError(
                    "CURRENT_MONTH_REVENUE_ERROR",
                    "An error occurred while calculating current month revenue. Please try again later."
                );
            }
        }

        /// <summary>
        /// Get revenue statistics by month for the last 12 months
        /// </summary>
        /// <returns>Revenue by month data</returns>
        public async Task<RevenueByMonthDto> GetRevenueByMonthAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                // Calculate start date (12 months ago from the beginning of current month)
                var startDate = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1).AddMonths(-11), DateTimeKind.Utc);
                // End date is the beginning of next month
                var endDate = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1).AddMonths(1), DateTimeKind.Utc);

                // Get revenue data by month from repository
                var monthlyData = await _enrollmentRepository.GetRevenueByMonthAsync(startDate, endDate);

                // Create a dictionary to store results
                var result = new Dictionary<string, decimal>();

                // Initialize all 12 months with 0 amount
                var currentMonth = startDate;
                for (int i = 0; i < 12; i++)
                {
                    var monthKey = currentMonth.ToString("MM/yyyy");
                    result[monthKey] = 0m;
                    currentMonth = currentMonth.AddMonths(1);
                }

                // Update with actual data from repository
                foreach (var monthData in monthlyData)
                {
                    var monthKey = $"{monthData.Month:D2}/{monthData.Year}";
                    result[monthKey] = monthData.TotalAmount;
                }

                return new RevenueByMonthDto
                {
                    Data = result,
                    Currency = "VND",
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex) when (!(ex is ApiException))
            {
                throw ApiException.InternalServerError(
                    "REVENUE_BY_MONTH_ERROR",
                    "An error occurred while calculating revenue by month. Please try again later."
                );
            }
        }
    }
}
