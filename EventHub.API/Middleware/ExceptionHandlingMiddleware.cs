using EventHub.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace EventHub.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";
            switch (exception)
            {
                case ValidationException valEx:
                    code = HttpStatusCode.BadRequest;
                    message = valEx.Message;
                    break;
                case NotFoundException notFoundEx:
                    code = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    message = "You are not authorized to perform this action.";
                    break;
                case ForbiddenException:
                    code = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                statusCode = (int)code,
                timestamp = DateTime.UtcNow
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
