using ArtAuctionHub.Domain.Interfaces.Repositories;

namespace ArtAuctionHub.API.Middlewares
{
    /// <summary>
    /// Middleware that checks for the presence of an 'auctionId' parameter in the route data.
    /// If present, it validates the value and ensures that an auction with the specified ID exists in the database.
    /// If the auction does not exist, the middleware short-circuits the pipeline and returns a 404 Not Found response.
    /// If the 'auctionId' is not a valid integer, it returns a 400 Bad Request response.
    /// Otherwise, the request is passed to the next middleware in the pipeline.
    /// 
    /// Usage:
    /// Register this middleware in the ASP.NET Core pipeline (e.g., in Program.cs) to automatically validate auction existence
    /// for any endpoint that includes 'auctionId' in its route.
    /// 
    /// Example:
    ///   app.UseMiddleware&lt;AuctionExistenceMiddleware&gt;();
    /// 
    /// This middleware is especially useful for protecting endpoints that operate on specific auctions,
    /// ensuring that invalid or non-existent auction IDs are handled consistently and early in the request lifecycle.
    /// 
    /// Dependencies:
    ///   - IAuctionRepository: Used to check for the existence of the auction in the data store.
    /// 
    /// Response Codes:
    ///   - 400 Bad Request: If 'auctionId' is present but not a valid integer.
    ///   - 404 Not Found: If 'auctionId' is valid but no auction exists with that ID.
    ///   - Proceeds to next middleware if auction exists or 'auctionId' is not present.
    /// </summary>
    public class AuctionExistenceMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionExistenceMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public AuctionExistenceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Processes HTTP requests to check for auction existence based on 'auctionId' in the route.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract route data from the current HTTP context
            var routeData = context.GetRouteData();
            var auctionIdString = routeData?.Values["auctionId"]?.ToString();

            // If 'auctionId' is present in the route, validate and check existence
            if (auctionIdString != null)
            {
                // Validate that 'auctionId' is a valid integer
                if (!int.TryParse(auctionIdString, out var auctionId))
                {
                    // Return 400 Bad Request if 'auctionId' is not a valid integer
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { message = "Invalid auctionId in route." });
                    return;
                }

                // Resolve the auction repository from the request's service provider
                var auctionRepository = context.RequestServices.GetRequiredService<IAuctionRepository>();

                // Check if an auction with the specified ID exists
                var auctionExists = await auctionRepository.AnyAsync(a => a.Id == auctionId);

                // If the auction does not exist, return 404 Not Found
                if (!auctionExists)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsJsonAsync(new { message = $"Auction with ID {auctionId} not found." });
                    return;
                }
            }

            // If 'auctionId' is not present or auction exists, continue processing the request
            await _next(context);
        }
    }
}