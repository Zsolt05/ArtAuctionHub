using ArtAuctionHub.API.Middlewares;

namespace ArtAuctionHub.API.Extensions
{
    /// <summary>
    /// Extension methods for registering and configuring custom middleware components.
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Adds the AuctionBidWebSocketMiddleware to the application's request pipeline.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAuctionBidWebSocket(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuctionBidWebSocketMiddleware>();
        }

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
}