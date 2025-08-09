namespace JCertPreApplication.Application.Dtos.AdminDashboard
{
    /// <summary>
    /// DTO for enrollments by month statistics
    /// </summary>
    public class EnrollmentsByMonthDto
    {
        /// <summary>
        /// Dictionary containing monthly enrollment data
        /// Key: "MM/yyyy" format (e.g., "08/2025")
        /// Value: Number of enrollments in that month
        /// </summary>
        public Dictionary<string, long> Data { get; set; } = new();

        /// <summary>
        /// Data calculation timestamp
        /// </summary>
        public DateTime CalculatedAt { get; set; }
    }
}
