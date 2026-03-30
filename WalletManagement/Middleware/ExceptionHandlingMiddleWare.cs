using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using WalletManagement.Core.Exceptions;

namespace WalletManagement.Middleware
{
    public class ExceptionHandlingMiddleWare : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleWare> _logger;

        public ExceptionHandlingMiddleWare(ILogger<ExceptionHandlingMiddleWare> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            // All catch blocks are now standardized to write a JSON response
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found.");
                await WriteProblemDetailsResponse(context, HttpStatusCode.NotFound, "Not Found", ex.Message);
            }
            catch (ServiceNotAvailableException ex)
            {
                _logger.LogError(ex, "Service is unavailable.");
                await WriteProblemDetailsResponse(context, HttpStatusCode.ServiceUnavailable, "Service Unavailable", ex.Message);
            }
            catch (GatewayTimeoutException ex)
            {
                _logger.LogError(ex, "Gateway timeout occurred.");
                await WriteProblemDetailsResponse(context, HttpStatusCode.GatewayTimeout, "Gateway Timeout", ex.Message);
            }
            catch (RequestTimeoutException ex)
            {
                _logger.LogError(ex, "Request timed out.");
                await WriteProblemDetailsResponse(context, HttpStatusCode.RequestTimeout, "Request Timeout", ex.Message);
            }
            catch (APIException ex)
            {
                _logger.LogError(ex, "API Exception occurred.");
                // Using 500 for a generic API exception, as 1001 is not a standard HTTP code.
                await WriteProblemDetailsResponse(context, HttpStatusCode.InternalServerError, "API Error", ex.Message, 1001);
            }
            catch (Exception ex)
            {
                // Catch any other unhandled exceptions and format them as a standard 500 error
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await WriteProblemDetailsResponse(context, HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");
            }
        }

        private async Task WriteProblemDetailsResponse(HttpContext context, HttpStatusCode statusCode, string title, string detail, int? customStatus = null)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = customStatus ?? (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}