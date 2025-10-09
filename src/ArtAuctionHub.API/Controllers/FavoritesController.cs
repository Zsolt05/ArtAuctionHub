using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Shared.Constants;
using ArtAuctionHub.Shared.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The FavoritesController manages a user's favorite artworks.
    /// </summary>
    /// <remarks>
    /// Constructor for FavoritesController.
    /// </remarks>
    /// <param name="favoriteService">An instance of IFavoriteService to handle favorite operations. Registered in the Dependency Injection (DI) container.</param>
    /// <param name="logger">The logger instance for logging favorite actions.</param>
    /// <param name="cacheService">The cache service instance for caching favorite data.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/favorites")] // Base route for favorites-related endpoints. (/api/favorites)
    [Authorize(Roles = RoleNames.Buyer)] // Only users with the "Buyer" role can access these endpoints.
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ILogger<FavoritesController> _logger;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for FavoritesController.
        /// </summary>
        /// <param name="favoriteService">Injected favorite service.</param>
        /// <param name="logger">Logger for favorite actions.</param>
        /// <param name="cacheService">Cache service for caching favorite data.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public FavoritesController(IFavoriteService favoriteService, 
            ILogger<FavoritesController> logger, ICacheService cacheService,
            IMapper mapper)
        {
            _favoriteService = favoriteService;
            _logger = logger;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds an artwork to the current user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to add.</param>
        /// <returns>A 200 OK response with a confirmation message.</returns>
        [HttpPost]
        [Route("{artworkId}")] // Matches POST /api/favorites/{artworkId}.
        public async Task<IActionResult> AddFavorite(int artworkId)
        {
            int userId = User.GetUserId();
            _logger.LogInformation("User {UserId} is adding artwork {ArtworkId} to favorites", userId, artworkId);
            await _favoriteService.AddFavoriteAsync(artworkId, userId);
            _logger.LogInformation("Artwork {ArtworkId} added to favorites for user {UserId}", artworkId, userId);

            _cacheService.Remove(CacheKeyNames.AllFavorites); // Invalidate the cache for all favorites to ensure data consistency.

            return Ok(new { Message = $"Artwork {artworkId} added to favorites." });
        }

        /// <summary>
        /// Removes an artwork from the user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to remove.</param>
        /// <returns>A 200 OK response with a confirmation message.</returns>
        [HttpDelete]
        [Route("{artworkId}")] // Matches DELETE /api/favorites/{artworkId}.
        public async Task<IActionResult> RemoveFavorite(int artworkId)
        {
            int userId = User.GetUserId();
            _logger.LogInformation("User {UserId} is removing artwork {ArtworkId} from favorites", userId, artworkId);
            await _favoriteService.RemoveFavoriteAsync(artworkId, userId);
            _logger.LogInformation("Artwork {ArtworkId} removed from favorites for user {UserId}", artworkId, userId);

            _cacheService.Remove(CacheKeyNames.AllFavorites); // Invalidate the cache for all favorites to ensure data consistency.

            return Ok(new { Message = $"Artwork {artworkId} removed from favorites." });
        }

        /// <summary>
        /// Retrieves all favorite artworks of the current user.
        /// </summary>
        /// <returns>A 200 OK response with the list of favorite artworks.</returns>
        [HttpGet] // Matches GET /api/favorites.
        public async Task<IActionResult> GetFavorites()
        {
            int userId = User.GetUserId();
            _logger.LogInformation("Retrieving favorites for user {UserId}", userId);
            var favorites = await _cacheService.GetOrCreateAsync(
                CacheKeyNames.AllFavorites,
                () => _favoriteService.GetFavoritesAsync(userId),
                absoluteExpireTime: TimeSpan.FromMinutes(30),
                slidingExpireTime: TimeSpan.FromMinutes(2)
            );

            var favoritesDto = _mapper.Map<List<ReadArtworkDto>>(favorites);

            _logger.LogInformation("Returned {Count} favorites for user {UserId}", favoritesDto.Count, userId);
            return Ok(favoritesDto); // Returns 200 OK with the list of favorite artworks.
        }
    }
}