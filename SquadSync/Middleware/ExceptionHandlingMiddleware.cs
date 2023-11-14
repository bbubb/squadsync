using SquadSync.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SquadSync.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is EntityNotFoundException) code = HttpStatusCode.NotFound;
            else if (exception is ArgumentException) code = HttpStatusCode.BadRequest;
            // More exception types can be added here

            var result = JsonSerializer.Serialize(new { error = exception.Message, stackTrace = _env.IsDevelopment() ? exception.StackTrace : null });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            _logger.LogError(exception, "An unhandled exception occurred.");

            return context.Response.WriteAsync(result);
        }
    }
}

