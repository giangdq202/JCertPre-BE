namespace JCertPreApplication.Application.Dtos.AdminDashboard
{
    /// <summary>
    /// DTO for current month revenue statistics
    /// </summary>
    public class CurrentMonthRevenueDto
    {
        /// <summary>
        /// Total revenue amount in current month
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Currency unit (e.g., VND, USD)
        /// </summary>
        public string Currency { get; set; } = "VND";

        /// <summary>
        /// Current month in MM/yyyy format
        /// </summary>
        public string Month { get; set; } = string.Empty;

        /// <summary>
        /// Data calculation timestamp
        /// </summary>
        public DateTime CalculatedAt { get; set; }
    }
}
