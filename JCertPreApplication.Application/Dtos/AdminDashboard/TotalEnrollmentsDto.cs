namespace JCertPreApplication.Application.Dtos.AdminDashboard
{
    /// <summary>
    /// DTO for total enrollments information
    /// </summary>
    public class TotalEnrollmentsDto
    {
        /// <summary>
        /// Total number of course enrollments in the system
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// Data calculation timestamp
        /// </summary>
        public DateTime CalculatedAt { get; set; }
    }
}
