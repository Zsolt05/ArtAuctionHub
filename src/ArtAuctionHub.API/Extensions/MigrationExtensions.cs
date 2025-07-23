using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.API.Extensions
{
    /// <summary>
    /// Extension methods for IApplicationBuilder to apply EF Core migrations automatically.
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Applies any pending EF Core migrations for the specified DbContext.
        /// </summary>
        /// <typeparam name="T">Type of the DbContext.</typeparam>
        /// <param name="app">IApplicationBuilder instance.</param>
        /// <returns>The IApplicationBuilder instance for chaining.</returns>
        public static IApplicationBuilder MigrateDatabase<T>(this IApplicationBuilder app) where T : DbContext
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<T>();
                dbContext.Database.Migrate(); // Applies all pending migrations
            }

            return app;
        }
    }
}