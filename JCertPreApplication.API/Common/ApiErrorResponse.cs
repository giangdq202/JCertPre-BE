namespace JCertPreApplication.API.Common
{
    /// <summary>
    /// 🚨 Standardized API error response structure.
    /// </summary>
    /// <remarks>
    /// This class provides a consistent error response format across all API endpoints.
    /// It includes error codes, messages, and optional details for debugging.
    /// </remarks>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Machine-readable error code for programmatic handling.
        /// </summary>
        /// <example>COURSE_NOT_FOUND</example>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Human-readable error message describing what went wrong.
        /// </summary>
        /// <example>The requested course could not be found.</example>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional error details for debugging (typically shown in development only).
        /// </summary>
        /// <example>Course with ID '3fa85f64-5717-4562-b3fc-2c963f66afa6' does not exist in the database.</example>
        public string? Details { get; set; }

        /// <summary>
        /// HTTP status code associated with this error.
        /// </summary>
        /// <example>404</example>
        public int StatusCode { get; set; }

        /// <summary>
        /// Timestamp when the error occurred (UTC).
        /// </summary>
        /// <example>2024-01-15T08:30:00Z</example>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Request path where the error occurred.
        /// </summary>
        /// <example>/api/Courses/3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
        public string? Path { get; set; }
    }
} 