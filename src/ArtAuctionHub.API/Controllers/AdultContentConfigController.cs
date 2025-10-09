using ArtAuctionHub.Domain.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// Demonstrates the use of <see cref="IOptionsSnapshot{TOptions}"/> for configuration.
    /// <para>
    /// <b>IOptionsSnapshot&lt;T&gt;:</b> This interface provides configuration values that are refreshed for every HTTP request.
    /// It is <b>scoped</b> to the request, so if the configuration source changes (e.g., appsettings.json is reloaded),
    /// the new values will be available on the next request.
    /// </para>
    /// <para>
    /// <b>When to use:</b> Use <c>IOptionsSnapshot&lt;T&gt;</c> in controllers or services that need to always use the latest configuration
    /// for each request, such as when an admin can change settings at runtime.
    /// </para>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdultContentConfigController : ControllerBase
    {
        private readonly AdultContentOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdultContentConfigController"/> class.
        /// </summary>
        /// <param name="optionsSnapshot">
        /// The <see cref="IOptionsSnapshot{AdultContentOptions}"/> instance injected by the DI container.
        /// </param>
        public AdultContentConfigController(IOptionsSnapshot<AdultContentOptions> optionsSnapshot)
        {
            // This value is refreshed for every HTTP request.
            _options = optionsSnapshot.Value;
        }

        /// <summary>
        /// Gets the current minimum age for adult content from the configuration.
        /// </summary>
        /// <returns>The minimum age as an integer.</returns>
        [HttpGet("minimum-age")]
        public IActionResult GetMinimumAge() => Ok(_options.MinimumAge);

        /// <summary>
        /// Gets the current forbidden message for adult content from the configuration.
        /// </summary>
        /// <returns>The forbidden message as a string.</returns>
        [HttpGet("forbidden-message")]
        public IActionResult GetForbiddenMessage() => Ok(_options.ForbiddenMessage);
    }
}