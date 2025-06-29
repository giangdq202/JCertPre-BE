namespace JCertPreApplication.API.Common
{
    /// <summary>
    /// Standard error response structure for all API errors.
    /// This ensures consistent error handling across the entire application.
    /// </summary>
    public record ApiErrorResponse(
        /// <summary>
        /// HTTP status code (e.g., 400, 401, 404, 500)
        /// </summary>
        int Status,
        
        /// <summary>
        /// Application-specific error code for easy debugging and client handling
        /// Examples: "USER_NOT_FOUND", "INVALID_CREDENTIALS", "VALIDATION_FAILED"
        /// </summary>
        string ErrorCode,
        
        /// <summary>
        /// Human-readable error message
        /// </summary>
        string Message,
        
        /// <summary>
        /// Trace ID for server-side error tracking (only for 500 errors)
        /// </summary>
        string? TraceId = null,
        
        /// <summary>
        /// Detailed validation errors (only for validation failures)
        /// Key: field name, Value: array of error messages for that field
        /// </summary>
        Dictionary<string, string[]>? Errors = null
    );
} 