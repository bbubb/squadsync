using Serilog;
using Serilog.Context;

namespace SquadSync.Middleware
{
    public class CorrelationIdMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Remove once invoke is confirmed
            Log.Information("CorrelationIdMiddleware: Invoked");

            // Check if the incoming request has a correlation ID, and if not, create one.
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers.Add(CorrelationIdHeaderName, correlationId);
            }

            // Put the correlation ID into the log context so that it is included in all logs.
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                // Add the correlation ID to the response header for client side tracking if needed.
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(CorrelationIdHeaderName, correlationId);
                    return Task.CompletedTask;
                });

                // Call the next middleware in the pipeline.
                await _next(context);
            }
        }
    }
}
