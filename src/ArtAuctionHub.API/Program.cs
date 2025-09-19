using ArtAuctionHub.API.Extensions;
using ArtAuctionHub.API.Filters;
using ArtAuctionHub.API.Middlewares;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Application.Services;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Infrastructure.Extensions;
using ArtAuctionHub.Infrastructure.Persistence;
using ArtAuctionHub.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// Creates a WebApplicationBuilder, which is used to configure
// services and middleware for the application.
// Think of it as the setup phase of your app.
var builder = WebApplication.CreateBuilder(args);

// Configures logging for the application.
// Clears any existing logging providers and adds console logging.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Adds JWT authentication services to the application.
builder.Services.AddJwtAuth(builder.Configuration);

// Adds database-related services, including the DbContext and repositories.
builder.Services.AddInfrastructure(builder.Configuration);

// Registers controllers as services in the dependency injection container.
// This enables us to create controllers (classes with [ApiController] attribute)
// that define REST endpoints.
// Adds a global action filter to validate models on all controllers.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
});

// Disables the automatic model state validation that ASP.NET Core does.
builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.SuppressModelStateInvalidFilter = true;
});

// Configures ASP.NET Core Identity for user and role management.
// This sets up the necessary services to handle user authentication,
// password hashing, role management, and more.
builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<ArtAuctionHubDbContext>()
    .AddDefaultTokenProviders();

// Adds Response Caching middleware to the application.
// This allows responses to be cached, improving performance for
// frequently requested resources.
builder.Services.AddResponseCaching();

// Adds in-memory caching services to the application.
builder.Services.AddMemoryCache();

// Registers application services in the dependency injection container.
// These services can be injected into controllers or other services.
// There are several services registered here, each responsible for a specific part of the application logic.

// 3 types of services are can be registered:
// 1. Scoped: Created once per request.
//    Example: IAuthService, IAuctionService, DbContext.
//    (These services maintain per-request data and are reused throughout a single HTTP request.)

// 2. Singleton: Created once and shared throughout the application.
//    Example: IHttpContextAccessor, ILogger<T>, IConfiguration.
//    (These services are created once and shared globally, as they don't depend on per-request data.)

// 3. Transient: Created new each time they are injected.
//    Example: IGuidGenerator, EmailSenderService, PasswordHasher.
//    (These are lightweight services that are often stateless and safe to recreate each time.)

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IArtworkService, ArtworkService>();
builder.Services.AddScoped<IBidService, BidService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<ICacheService, MemoryCacheService>();

builder.Services.AddScoped<PasswordHasherService>();

builder.Services.AddExceptionHandling();
// Builds the WebApplication object from the builder.
// This is the main object used to configure the app's request pipeline
// and define routes, middleware, and more.
var app = builder.Build();

// Adds custom exception handling middleware to the request pipeline.
// This middleware will catch unhandled exceptions and convert them
// into standardized HTTP error responses.
app.UseExceptionHandling();

// Adds WebSocket support to the application.
app.UseWebSockets();

// Adds custom request logging middleware to the request pipeline.
app.UseRequestLoggingMiddleware();

// Adds middleware that redirects HTTP requests to HTTPS.
// This improves security by ensuring encrypted communication.
// Example: http://localhost:5000 → https://localhost:7000
app.UseHttpsRedirection();

// Adds response caching middleware to the request pipeline.
app.UseResponseCaching();

// Adds authentication middleware to the request pipeline.
app.UseAuthentication();
// Adds authorization middleware to the request pipeline.
app.UseAuthorization();

// Adds AuctionBid WebSocket middleware to the request pipeline.
// This middleware handles WebSocket connections for real-time auction bidding.
// You can test WebSocket connections using tools like "Quick WebSocket Client" Chrome extension:
// https://chromewebstore.google.com/detail/quick-websocket-client-wi/enmpedlkjjjnhoehlkkghdjiloebecpn
// Example WebSocket URL: wss://localhost:7000/ws/auction-bids?auctionId=4&access_token=eyJhbG..
app.UseAuctionBidWebSocket();

// Maps all controller endpoints to the request pipeline.
// If we have controllers (e.g., AuthController), 
// their routes will be automatically recognized and served.
app.MapControllers();

// An array of weather condition summaries.
// These strings will be used randomly to describe each forecast.
var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching"
};

// Maps a GET endpoint at "/weatherforecast".
// When you access this URL in the browser or via a tool like Postman,
// it will return an array of weather forecasts in JSON format.
app.MapGet("/weatherforecast", () =>
{
    // Generate 5 forecast entries for the next 5 days.
    // Each forecast includes:
    // - a future date
    // - a random temperature in Celsius (-20 to 55)
    // - a random weather summary from the list above
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;

    // Example response (JSON):
    // [
    //   { "date": "2025-07-04", "temperatureC": 22, "summary": "Warm", "temperatureF": 71 },
    //   { "date": "2025-07-05", "temperatureC": -2, "summary": "Freezing", "temperatureF": 28 },
    //   ...
    // ]
})
.WithName("GetWeatherForecast"); // This names the endpoint (useful for documentation or linking)

// This line adds automatic update database migrations.
app.MigrateDatabase<ArtAuctionHubDbContext>();

// Starts the application and begins listening for HTTP requests.
// This is the final step where the app becomes active.
await app.RunAsync();

/// <summary>
/// Represents a single day's weather forecast.
/// This will be automatically converted to JSON by ASP.NET when returned from the API.
/// </summary>
/// <param name="Date">The date of the forecast (only the day, not time).</param>
/// <param name="TemperatureC">Temperature in Celsius.</param>
/// <param name="Summary">Textual weather description like "Chilly" or "Hot".</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// The temperature in Fahrenheit.
    /// Formula: F = 32 + (C / 0.5556)
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}