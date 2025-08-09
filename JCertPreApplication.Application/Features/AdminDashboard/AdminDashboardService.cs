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

        public AdminDashboardService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
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
                var moneyPayments = await _paymentRepository.GetPaymentsByTypeAsync(PaymentType.Money);

                // Filter by successful payments only
                var successfulPayments = moneyPayments
                    .Where(p => p.status == PaymentStatus.Completed)
                    .ToList();

                // Calculate total amount
                var totalAmount = successfulPayments.Sum(p => p.amount);

                return new TotalRevenueDto
                {
                    TotalAmount = totalAmount,
                    Currency = "VND",
                    TotalTransactions = successfulPayments.Count,
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
    }
}
