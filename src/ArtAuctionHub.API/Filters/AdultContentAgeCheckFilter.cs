using ArtAuctionHub.Domain.Interfaces.Repositories;
using ArtAuctionHub.Domain.Options;
using ArtAuctionHub.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ArtAuctionHub.API.Filters
{
    /// <summary>
    /// An action filter that checks if a user is old enough to interact with adult-only artworks.
    /// Prevents users under the configured minimum age from favoriting or accessing adult content.
    /// <para>
    /// This filter demonstrates the use of <see cref="IOptionsSnapshot{TOptions}"/> for configuration.
    /// <b>IOptionsSnapshot&lt;T&gt;:</b> This interface provides configuration values that are refreshed for every HTTP request.
    /// It is <b>scoped</b> to the request, so if the configuration source changes (e.g., appsettings.json is reloaded),
    /// the new values will be available on the next request.
    /// </para>
    /// <para>
    /// <b>When to use:</b> Use <c>IOptionsSnapshot&lt;T&gt;</c> in filters, controllers, or services that need to always use the latest configuration
    /// for each request, such as when an admin can change settings at runtime.
    /// </para>
    /// </summary>
    public class AdultContentAgeCheckFilter : IAsyncActionFilter
    {
        private readonly IUserRepository _userRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly ILogger<AdultContentAgeCheckFilter> _logger;
        private readonly AdultContentOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdultContentAgeCheckFilter"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository for retrieving user data.</param>
        /// <param name="artworkRepository">The artwork repository for retrieving artwork data.</param>
        /// <param name="logger">The logger instance for logging filter actions.</param>
        /// <param name="optionsSnapshot">
        /// The <see cref="IOptionsSnapshot{AdultContentOptions}"/> instance injected by the DI container.
        /// Provides the latest configuration for each HTTP request.
        /// </param>
        public AdultContentAgeCheckFilter(
            IUserRepository userRepository,
            IArtworkRepository artworkRepository,
            ILogger<AdultContentAgeCheckFilter> logger,
            IOptionsSnapshot<AdultContentOptions> optionsSnapshot)
        {
            _userRepository = userRepository;
            _artworkRepository = artworkRepository;
            _logger = logger;
            _options = optionsSnapshot.Value;
        }

        /// <summary>
        /// Executes the age check logic before the action executes.
        /// If the artwork is adult-only and the user is under the configured minimum age, forbids the action.
        /// </summary>
        /// <param name="context">The action executing context.</param>
        /// <param name="next">The delegate to execute the next action filter or action.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get artworkId from action arguments
            if (!context.ActionArguments.TryGetValue("artworkId", out var artworkIdObj) || artworkIdObj is not int artworkId)
            {
                context.Result = new BadRequestObjectResult("Invalid artworkId.");
                return;
            }

            // Get userId from ClaimsPrincipal
            int userId;
            try
            {
                userId = context.HttpContext.User.GetUserId();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve user ID from claims.");
                context.Result = new UnauthorizedResult();
                return;
            }

            // Retrieve user and artwork from repositories
            var user = await _userRepository.GetByIdAsync(userId);
            var artwork = await _artworkRepository.FindByIdAsync(artworkId);

            // If either user or artwork is not found, return 404 NotFound
            if (user == null || artwork == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            // If the artwork is adult-only, check user's age
            if (artwork.IsAdultOnly)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var age = today.Year - user.BirthDate.Year;
                // Adjust age if the user's birthday hasn't occurred yet this year
                if (user.BirthDate > today.AddYears(-age)) age--;

                // If user is under the configured minimum age, forbid the action
                if (age < _options.MinimumAge)
                {
                    context.Result = new ObjectResult(_options.ForbiddenMessage)
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                    return;
                }
            }

            // Continue to the next action filter or action
            await next();
        }
    }
}