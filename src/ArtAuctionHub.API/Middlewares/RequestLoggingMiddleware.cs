using System.Diagnostics;

namespace ArtAuctionHub.API.Middlewares
{
    /// <summary>
    /// Middleware for logging HTTP request and response details, including method, path, status code, and elapsed time.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to log request and response information.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Handling request: {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

            await _next(httpContext);

            stopwatch.Stop();
            _logger.LogInformation(
                "Finished handling request. Status Code: {StatusCode}. Elapsed Time: {ElapsedMilliseconds} ms",
                httpContext.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Extension methods for adding the <see cref="RequestLoggingMiddleware"/> to the application pipeline.
    /// </summary>
    public static class RequestLoggingMiddlewareExtensions
    {
        /// <summary>
        /// Adds the <see cref="RequestLoggingMiddleware"/> to the application's request pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}