namespace JCertPreApplication.Application.Dtos.AdminDashboard
{
    /// <summary>
    /// DTO for revenue by month statistics
    /// </summary>
    public class RevenueByMonthDto
    {
        /// <summary>
        /// Dictionary containing monthly revenue data
        /// Key: "MM/yyyy" format (e.g., "08/2025")
        /// Value: Revenue amount in that month
        /// </summary>
        public Dictionary<string, decimal> Data { get; set; } = new();

        /// <summary>
        /// Currency unit (e.g., VND, USD)
        /// </summary>
        public string Currency { get; set; } = "VND";

        /// <summary>
        /// Data calculation timestamp
        /// </summary>
        public DateTime CalculatedAt { get; set; }
    }
}
