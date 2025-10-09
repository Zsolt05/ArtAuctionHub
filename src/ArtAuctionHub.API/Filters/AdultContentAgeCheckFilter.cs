using ArtAuctionHub.Domain.Interfaces.Repositories;
using ArtAuctionHub.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ArtAuctionHub.API.Filters
{
    /// <summary>
    /// An action filter that checks if a user is old enough to interact with adult-only artworks.
    /// Prevents users under 18 from favoriting or accessing adult content.
    /// </summary>
    public class AdultContentAgeCheckFilter : IAsyncActionFilter
    {
        private readonly IUserRepository _userRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly ILogger<AdultContentAgeCheckFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdultContentAgeCheckFilter"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository for retrieving user data.</param>
        /// <param name="artworkRepository">The artwork repository for retrieving artwork data.</param>
        /// <param name="logger">The logger instance for logging filter actions.</param>
        public AdultContentAgeCheckFilter(IUserRepository userRepository,
            IArtworkRepository artworkRepository,
            ILogger<AdultContentAgeCheckFilter> logger)
        {
            _userRepository = userRepository;
            _artworkRepository = artworkRepository;
            _logger = logger;
        }

        /// <summary>
        /// Executes the age check logic before the action executes.
        /// If the artwork is adult-only and the user is under 18, forbids the action.
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

                // If user is under 18, forbid the action
                if (age < 18)
                {
                    context.Result = new ObjectResult("You must be at least 18 years old to favorite adult content.")
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