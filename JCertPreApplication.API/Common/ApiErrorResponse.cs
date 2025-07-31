namespace JCertPreApplication.API.Common
{
    /// <summary>
    /// Standardized API error response structure.
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Machine-readable error code.
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional error details for debugging.
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp when the error occurred.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Request path where the error occurred.
        /// </summary>
        public string? Path { get; set; }
    }
} 