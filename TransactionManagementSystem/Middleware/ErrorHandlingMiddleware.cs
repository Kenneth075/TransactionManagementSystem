using System.Text.Json;
using TransactionManagementSystem.Domain.Exceptions;

namespace TransactionManagementSystem.API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var result = exception switch
            {
                InsufficientFundsException => new
                {
                    error = "Insufficient funds",
                    message = exception.Message,
                    statusCode = 400
                },
                AccountNotFoundException => new
                {
                    error = "Account not found",
                    message = exception.Message,
                    statusCode = 404
                },
                InvalidTransactionException => new
                {
                    error = "Invalid transaction",
                    message = exception.Message,
                    statusCode = 400
                },
                UnauthorizedAccessException => new
                {
                    error = "Unauthorized",
                    message = "Access denied",
                    statusCode = 401
                },
                _ => new
                {
                    error = "Internal server error",
                    message = "An error occurred while processing your request",
                    statusCode = 500
                }
            };

            response.StatusCode = result.statusCode;
            await response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }

}

