using ArtAuctionHub.Domain.Options;
using Microsoft.Extensions.Options;

namespace ArtAuctionHub.API.Services
{
    /// <summary>
    /// Demonstrates the use of <see cref="IOptionsMonitor{TOptions}"/> for configuration.
    /// <para>
    /// <b>IOptionsMonitor&lt;T&gt;:</b> This interface provides the latest configuration values at any time and allows you to react to changes.
    /// It is a <b>singleton</b> and supports change notifications, so you can subscribe to configuration changes and update your logic or cache accordingly.
    /// </para>
    /// <para>
    /// <b>When to use:</b> Use <c>IOptionsMonitor&lt;T&gt;</c> in background services, caches, or any singleton that needs to respond to config changes at runtime.
    /// </para>
    /// </summary>
    public class AdultContentChangeWatcher
    {
        private AdultContentOptions _current;
        private readonly ILogger<AdultContentChangeWatcher> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdultContentChangeWatcher"/> class.
        /// Subscribes to configuration changes and logs when the minimum age changes.
        /// </summary>
        /// <param name="monitor">
        /// The <see cref="IOptionsMonitor{AdultContentOptions}"/> instance injected by the DI container.
        /// </param>
        /// <param name="logger">
        /// The logger used to log configuration changes.
        /// </param>
        public AdultContentChangeWatcher(IOptionsMonitor<AdultContentOptions> monitor, ILogger<AdultContentChangeWatcher> logger)
        {
            _logger = logger;
            _current = monitor.CurrentValue;

            // Subscribe to configuration changes. This delegate will be called whenever the config changes at runtime.
            monitor.OnChange(options =>
            {
                _logger.LogInformation("AdultContent config changed! New MinimumAge: {MinimumAge}", options.MinimumAge);
                _logger.LogInformation("AdultContent config changed! New ForbiddenMessage: {ForbiddenMessage}", options.ForbiddenMessage);
                _current = options;
            });
        }

        /// <summary>
        /// Gets the current minimum age for adult content from the latest configuration.
        /// </summary>
        /// <returns>The minimum age as an integer.</returns>
        public int GetCurrentMinimumAge() => _current.MinimumAge;

        /// <summary>
        /// Gets the current forbidden message for adult content from the latest configuration.
        /// </summary>
        /// <returns>The forbidden message as a string.</returns>
        public string GetCurrentForbiddenMessage() => _current.ForbiddenMessage;
    }
}