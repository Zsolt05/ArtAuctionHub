// -----------------------------------------------------------
// This is a simple ASP.NET Core Web API that exposes a 
// "/weatherforecast" endpoint. It returns a list of 
// randomly generated weather forecasts for the next 5 days.
// -----------------------------------------------------------

// Creates a WebApplicationBuilder, which is used to configure
// services and middleware for the application.
// Think of it as the setup phase of your app.
var builder = WebApplication.CreateBuilder(args);

// Builds the WebApplication object from the builder.
// This is the main object used to configure the app's request pipeline
// and define routes, middleware, and more.
var app = builder.Build();

// Adds middleware that redirects HTTP requests to HTTPS.
// This improves security by ensuring encrypted communication.
// Example: http://localhost:5000 → https://localhost:7000
app.UseHttpsRedirection();

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

// Starts the application and begins listening for HTTP requests.
// This is the final step where the app becomes active.
app.Run();

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