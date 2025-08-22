namespace ArtAuctionHub.API.Middlewares;

/// <summary>
/// Extension methods to add and use the ExceptionHandlingMiddleware.
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Registers the ExceptionHandlingMiddleware in the service collection.
    /// </summary>
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();
        return services;
    }

    /// <summary>
    /// Adds the ExceptionHandlingMiddleware to the application's request pipeline.
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}