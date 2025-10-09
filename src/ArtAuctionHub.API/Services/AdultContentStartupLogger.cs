using ArtAuctionHub.Domain.Options;
using Microsoft.Extensions.Options;

namespace ArtAuctionHub.API.Services
{
    /// <summary>
    /// Demonstrates the use of <see cref="IOptions{TOptions}"/> for configuration.
    /// <para>
    /// <b>IOptions&lt;T&gt;:</b> This interface is used to access configuration values that are read once at application startup.
    /// The values are fixed for the application's lifetime and do not change even if the configuration source changes.
    /// </para>
    /// <para>
    /// <b>When to use:</b> Use <c>IOptions&lt;T&gt;</c> when you need static, never-changing configuration values.
    /// This is ideal for singleton/background services that do not require live updates.
    /// </para>
    /// </summary>
    public class AdultContentStartupLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdultContentStartupLogger"/> class.
        /// Logs the configuration values for adult content options at application startup.
        /// </summary>
        /// <param name="options">
        /// The <see cref="IOptions{AdultContentOptions}"/> instance injected by the DI container.
        /// </param>
        /// <param name="logger">
        /// The logger used to log the configuration values.
        /// </param>
        public AdultContentStartupLogger(IOptions<AdultContentOptions> options, ILogger<AdultContentStartupLogger> logger)
        {
            // The values here are fixed for the application's lifetime.
            logger.LogInformation(
                "Startup config: MinimumAge={MinimumAge}, ForbiddenMessage={ForbiddenMessage}",
                options.Value.MinimumAge, options.Value.ForbiddenMessage);

            // Example: If you change the config file and do not restart the app, these values will NOT update.
        }
    }
}