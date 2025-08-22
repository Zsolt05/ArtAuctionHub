using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security;
using System.Text.Json;
using ArtAuctionHub.Shared.Exceptions;

namespace ArtAuctionHub.API.Middlewares
{
    /// <summary>
    /// ASP.NET Core middleware that provides centralized exception handling for the API.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This component catches all unhandled exceptions thrown by downstream middlewares and MVC,
    /// maps them to appropriate HTTP status codes, and returns an RFC 7807 compatible
    /// <c>application/problem+json</c> response body.
    /// </para>
    /// <para>
    /// It also logs the exception with a correlation <see cref="TraceId"/> and exposes that trace id
    /// to clients in the response payload to simplify troubleshooting.
    /// </para>
    /// <para>
    /// Place this middleware as close to the top of the pipeline as possible so it can intercept
    /// exceptions from every subsequent component.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var builder = WebApplication.CreateBuilder(args);
    /// builder.Services.AddTransient&lt;ExceptionHandlingMiddleware&gt;();
    ///
    /// var app = builder.Build();
    ///
    /// // Put it very early in the pipeline:
    /// app.UseMiddleware&lt;ExceptionHandlingMiddleware&gt;();
    ///
    /// app.MapControllers();
    /// app.Run();
    /// </code>
    /// </example>
    /// <seealso cref="NotFoundException"/>
    /// <seealso cref="ValidationException"/>
    public sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        /// <summary>
        /// Logger used to record handled and unhandled exceptions.
        /// </summary>
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Host environment, used to control the level of detail (e.g., stack traces) exposed in responses.
        /// </summary>
        private readonly IHostEnvironment _env;

        /// <summary>
        /// Creates a new <see cref="ExceptionHandlingMiddleware"/>.
        /// </summary>
        /// <param name="logger">The logger instance to write structured error logs.</param>
        /// <param name="env">The hosting environment used to tailor error details by environment.</param>
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>A task that completes when downstream processing is finished.</returns>
        /// <remarks>
        /// This method wraps downstream execution in a <c>try/catch</c>. Any exception is routed to
        /// <see cref="HandleExceptionAsync(HttpContext, Exception)"/> for logging and response rendering.
        /// </remarks>
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

        /// <summary>
        /// Logs the exception, maps it to an HTTP status code and a problem-details payload,
        /// and writes an <c>application/problem+json</c> response to the client.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <returns>A task that completes when the response has been written.</returns>
        /// <remarks>
        /// <para>
        /// The response follows RFC 7807 conventions and includes a <c>traceId</c> field that matches
        /// <see cref="Activity.Current"/> (if available) or <see cref="HttpContext.TraceIdentifier"/>.
        /// </para>
        /// <para>
        /// In development environments (<see cref="IHostEnvironment.IsDevelopment"/>), the <c>detail</c>
        /// property contains the full exception including stack trace; in non-development environments a
        /// generic message is returned for 5xx errors.
        /// </para>
        /// </remarks>
        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var (status, title, code, errors) = MapException(exception);

            // Correlated logging (trace id surfaces both in logs and response)
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            using (_logger.BeginScope(new Dictionary<string, object?> { ["TraceId"] = traceId, ["ErrorCode"] = code }))
            {
                if (status >= StatusCodes.Status500InternalServerError)
                {
                    _logger.LogError(exception, "Unhandled exception ({Code})", code);
                }
                else
                {
                    _logger.LogWarning(exception, "Handled domain/application exception ({Code})", code);
                }
            }

            var problem = new ProblemPayload
            {
                Type = $"https://httpstatuses.com/{status}",
                Title = title,
                Status = status,
                Detail = status >= StatusCodes.Status500InternalServerError
                    ? (_env.IsDevelopment() ? exception.ToString() : "Unexpected server error.")
                    : exception.Message,
                Instance = httpContext.Request.Path,
                TraceId = traceId,
                Code = code,
                Errors = errors
            };

            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problem, JsonOptions);
            await httpContext.Response.WriteAsync(json);
        }

        /// <summary>
        /// Shared JSON serializer options for problem payloads.
        /// </summary>
        /// <remarks>
        /// Uses camelCase naming, omits <c>null</c> properties, and avoids extra whitespace to reduce payload size.
        /// </remarks>
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        /// <summary>
        /// Maps a .NET exception instance to an HTTP status code, a human-readable title,
        /// a stable machine-readable error code, and (optionally) a field error dictionary.
        /// </summary>
        /// <param name="ex">The exception to map.</param>
        /// <returns>
        /// A tuple of:
        /// <list type="number">
        /// <item><description><c>status</c> – HTTP status code to return.</description></item>
        /// <item><description><c>title</c> – Short, human-readable error title.</description></item>
        /// <item><description><c>code</c> – Stable application error code for client-side branching.</description></item>
        /// <item><description><c>errors</c> – Optional dictionary of field errors for validation scenarios.</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// Current mappings:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// <see cref="ValidationException"/> → <c>400</c> (<c>validation_error</c>) with a single-message
        /// <c>errors["validation"]</c> entry. This uses <see cref="ValidationException.Message"/> because
        /// the BCL validation exception does not carry a field error collection.
        /// </description>
        /// </item>
        /// <item><description><see cref="NotFoundException"/> → <c>404</c> (<c>not_found</c>)</description></item>
        /// <item><description><see cref="UnauthorizedAccessException"/> → <c>401</c> (<c>unauthorized</c>)</description></item>
        /// <item><description><see cref="SecurityException"/> → <c>403</c> (<c>forbidden</c>)</description></item>
        /// <item><description><see cref="InvalidOperationException"/> → <c>409</c> (<c>conflict</c>)</description></item>
        /// <item><description>All others → <c>500</c> (<c>internal_server_error</c>)</description></item>
        /// </list>
        /// Extend this mapping to include additional domain-specific exceptions as needed.
        /// </remarks>
        private static (int status, string title, string code, IDictionary<string, string[]>? errors)
            MapException(Exception ex)
        {
            return ex switch
            {
                ValidationException ve => (
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    "validation_error",
                    ve.Data["errors"] as IDictionary<string, string[]>
                        ?? new Dictionary<string, string[]>
                        {
                            ["validation"] = [ve.Message]
                        }
                ),

                NotFoundException => (
                    StatusCodes.Status404NotFound,
                    "Resource not found",
                    "not_found",
                    null
                ),

                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "unauthorized",
                    null
                ),

                SecurityException => (
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    "forbidden",
                    null
                ),

                // Typical business rule collision or illegal state transition
                InvalidOperationException => (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "conflict",
                    null
                ),

                // Fallback for any unclassified exception
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "internal_server_error",
                    null
                )
            };
        }

        /// <summary>
        /// Minimal RFC 7807-like problem details payload that avoids taking a dependency
        /// on ASP.NET Core MVC abstractions.
        /// </summary>
        /// <remarks>
        /// This intentionally mirrors the structure of <c>ProblemDetails</c> while keeping the API
        /// project lightweight. Clients should treat <see cref="Code"/> as a stable, documented
        /// programmatic error identifier.
        /// </remarks>
        private sealed class ProblemPayload
        {
            /// <summary>
            /// A URI reference that identifies the problem type.
            /// </summary>
            /// <remarks>
            /// When dereferenced, this URI may provide human-readable documentation for the problem.
            /// Defaults to a public HTTP status documentation URL (e.g., <c>https://httpstatuses.com/404</c>).
            /// </remarks>
            public string? Type { get; set; }

            /// <summary>
            /// A short, human-readable summary of the problem type.
            /// </summary>
            public string? Title { get; set; }

            /// <summary>
            /// The HTTP status code generated by the origin server for this occurrence of the problem.
            /// </summary>
            public int? Status { get; set; }

            /// <summary>
            /// A human-readable explanation specific to this occurrence of the problem.
            /// </summary>
            /// <remarks>
            /// In development, for 5xx errors this may include the full exception (stack trace).
            /// In non-development environments a generic message is returned for 5xx errors.
            /// </remarks>
            public string? Detail { get; set; }

            /// <summary>
            /// A URI reference that identifies the specific occurrence of the problem.
            /// </summary>
            /// <remarks>Populated with the request path of the current HTTP request.</remarks>
            public string? Instance { get; set; }

            /// <summary>
            /// Correlation identifier for this error, surfaced to help correlate client errors with server logs.
            /// </summary>
            /// <remarks>
            /// Uses <see cref="Activity.Id"/> if present (W3C Trace-Context), otherwise
            /// <see cref="HttpContext.TraceIdentifier"/>.
            /// </remarks>
            public string? TraceId { get; set; }

            /// <summary>
            /// Stable, programmatic error code suitable for client-side branching and localization.
            /// </summary>
            /// <example>not_found</example>
            /// <example>validation_error</example>
            /// <example>internal_server_error</example>
            public string? Code { get; set; }

            /// <summary>
            /// Optional field-level error dictionary for validation scenarios.
            /// </summary>
            /// <remarks>
            /// Keys typically represent field or parameter names; values contain one or more error messages per field.
            /// When using <see cref="ValidationException"/>, this middleware returns a single entry with the key
            /// <c>"validation"</c> because the BCL exception does not provide per-field errors.
            /// </remarks>
            public IDictionary<string, string[]>? Errors { get; set; }
        }
    }
}