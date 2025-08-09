namespace JCertPreApplication.Application.Dtos.AdminDashboard
{
    /// <summary>
    /// DTO for current month enrollments statistics
    /// </summary>
    public class CurrentMonthEnrollmentsDto
    {
        /// <summary>
        /// Number of enrollments in current month
        /// </summary>
        public long Count { get; set; }

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
