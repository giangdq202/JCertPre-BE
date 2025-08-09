namespace JCertPreApplication.Application.Dtos.AdminDashboard
{
    /// <summary>
    /// DTO for total revenue information
    /// </summary>
    public class TotalRevenueDto
    {
        /// <summary>
        /// Total amount of money deposited to the system
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Currency unit (e.g., VND, USD)
        /// </summary>
        public string Currency { get; set; } = "VND";

        /// <summary>
        /// Total number of money deposit transactions
        /// </summary>
        public int TotalTransactions { get; set; }

        /// <summary>
        /// Data calculation timestamp
        /// </summary>
        public DateTime CalculatedAt { get; set; }
    }
}
