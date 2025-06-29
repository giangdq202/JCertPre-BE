using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Exceptions;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace JCertPreApplication.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware that catches all exceptions
    /// and converts them to standardized API error responses.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            ApiErrorResponse response;

            switch (exception)
            {
                // Business logic errors that we intentionally throw
                case ApiException apiEx:
                    context.Response.StatusCode = (int)apiEx.StatusCode;
                    response = new ApiErrorResponse(
                        Status: (int)apiEx.StatusCode,
                        ErrorCode: apiEx.ErrorCode,
                        Message: apiEx.Message,
                        Errors: apiEx.ValidationErrors
                    );
                    
                    // Log business logic errors at Warning level (not critical)
                    _logger.LogWarning("API Exception: {ErrorCode} - {Message}", 
                        apiEx.ErrorCode, apiEx.Message);
                    break;

                // All other unexpected system errors
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

                    response = new ApiErrorResponse(
                        Status: (int)HttpStatusCode.InternalServerError,
                        ErrorCode: "INTERNAL_SERVER_ERROR",
                        Message: "An unexpected error has occurred. Please contact support with the trace ID.",
                        TraceId: traceId
                    );
                    
                    // Log system errors at Error level with full details
                    _logger.LogError(exception, 
                        "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}", 
                        traceId, context.Request.Path, context.Request.Method);
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
} 