using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                KeyNotFoundException =>
                    (StatusCodes.Status404NotFound, "Resource not found"),
                ArgumentException =>
                    (StatusCodes.Status400BadRequest, "Invalid request"),
                InvalidOperationException =>
                    (StatusCodes.Status409Conflict, "Operation conflict"),
                _ =>
                    (StatusCodes.Status500InternalServerError, "Internal server error")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "An unexpected error occurred.");
            }
            else
            {
                _logger.LogWarning(
                    "Request failed with status code {StatusCode}: {Message}",
                    statusCode,
                    exception.Message);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
    }
}
