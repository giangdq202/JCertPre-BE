using System.Net;

namespace JCertPreApplication.Application.Exceptions
{
    /// <summary>
    /// Custom exception for API-related errors that should be returned to clients.
    /// This is the only exception you should throw intentionally in your business logic.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// HTTP status code to be returned to the client
        /// </summary>
        public HttpStatusCode StatusCode { get; }
        
        /// <summary>
        /// Application-specific error code for debugging and client handling
        /// </summary>
        public string ErrorCode { get; }
        
        /// <summary>
        /// Validation errors (only for validation failures)
        /// </summary>
        public Dictionary<string, string[]>? ValidationErrors { get; }

        /// <summary>
        /// Constructor for general API errors
        /// </summary>
        /// <param name="statusCode">HTTP status code (400, 401, 404, etc.)</param>
        /// <param name="errorCode">Application error code (e.g., "USER_NOT_FOUND")</param>
        /// <param name="message">Human-readable error message</param>
        public ApiException(HttpStatusCode statusCode, string errorCode, string message) 
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructor specifically for validation errors (always returns 400 Bad Request)
        /// </summary>
        /// <param name="errorCode">Application error code (e.g., "VALIDATION_FAILED")</param>
        /// <param name="message">General validation error message</param>
        /// <param name="validationErrors">Dictionary of field-specific validation errors</param>
        public ApiException(string errorCode, string message, Dictionary<string, string[]> validationErrors) 
            : base(message)
        {
            StatusCode = HttpStatusCode.BadRequest;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
        }

        // Convenience methods for common error types
        
        /// <summary>
        /// Creates a 404 Not Found exception
        /// </summary>
        public static ApiException NotFound(string resourceName, object key)
        {
            return new ApiException(
                HttpStatusCode.NotFound,
                "RESOURCE_NOT_FOUND",
                $"{resourceName} with key '{key}' was not found."
            );
        }

        /// <summary>
        /// Creates a 400 Bad Request exception
        /// </summary>
        public static ApiException BadRequest(string errorCode, string message)
        {
            return new ApiException(HttpStatusCode.BadRequest, errorCode, message);
        }

        /// <summary>
        /// Creates a 401 Unauthorized exception with default error code
        /// </summary>
        public static ApiException Unauthorized(string message = "Authentication required.")
        {
            return new ApiException(HttpStatusCode.Unauthorized, "UNAUTHORIZED", message);
        }

        /// <summary>
        /// Creates a 401 Unauthorized exception with custom error code
        /// </summary>
        public static ApiException Unauthorized(string errorCode, string message)
        {
            return new ApiException(HttpStatusCode.Unauthorized, errorCode, message);
        }

        /// <summary>
        /// Creates a 403 Forbidden exception with default error code
        /// </summary>
        public static ApiException Forbidden(string message = "Access forbidden.")
        {
            return new ApiException(HttpStatusCode.Forbidden, "FORBIDDEN", message);
        }

        /// <summary>
        /// Creates a 403 Forbidden exception with custom error code
        /// </summary>
        public static ApiException Forbidden(string errorCode, string message)
        {
            return new ApiException(HttpStatusCode.Forbidden, errorCode, message);
        }

        /// <summary>
        /// Creates a 500 Internal Server Error exception
        /// </summary>
        public static ApiException InternalServerError(string errorCode, string message)
        {
            return new ApiException(HttpStatusCode.InternalServerError, errorCode, message);
        }
    }
} 