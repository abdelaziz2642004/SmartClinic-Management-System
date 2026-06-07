using System.Diagnostics;

namespace Clinic.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Handling Request : {method} {Path} {Query}", context.Request.Method, context.Request.Path,context.Request.QueryString);
            await _next(context);
            stopwatch.Stop();
            _logger.LogInformation("Response : {method} {Path} {Query} in {ElapsedMilliseconds} ms", context.Request.Method, context.Request.Path, context.Request.QueryString, stopwatch.ElapsedMilliseconds);
        }
    }
}
