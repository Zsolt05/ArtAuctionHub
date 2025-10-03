using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Shared.Constants;
using ArtAuctionHub.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The ArtworksController manages CRUD operations and queries related to artworks.
    /// </summary>
    /// <remarks>
    /// Constructor for ArtworksController.
    /// </remarks>
    /// <param name="artworkService">An instance of IArtworkService to handle artwork operations. Registered in the Dependency Injection (DI) container.</param>
    /// <param name="logger">The logger instance for logging artwork actions.</param>
    /// <param name="cacheService">The cache service instance for caching artwork data.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/artworks")] // Base route for all artwork-related endpoints. (/api/artworks)
    [Authorize]
    public class ArtworksController : ControllerBase
    {
        private readonly IArtworkService _artworkService;
        private readonly ILogger<ArtworksController> _logger;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Constructor for ArtworksController.
        /// </summary>
        /// <param name="artworkService">Injected artwork service.</param>
        /// <param name="logger">Logger for artwork actions.</param>
        /// <param name="cacheService">Injected cache service.</param>
        public ArtworksController(IArtworkService artworkService, ILogger<ArtworksController> logger, ICacheService cacheService)
        {
            _artworkService = artworkService;
            _logger = logger;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Creates a new artwork and uploads an image.
        /// </summary>
        /// <param name="dto">The artwork data.</param>
        /// <param name="imageFile">The image file to be uploaded.</param>
        /// <returns>Returns the created artwork data.</returns>
        [HttpPost]
        [Authorize(Roles = RoleNames.Artist)] // Only users with the "Artist" role can access this endpoint.
        // POST /api/artworks
        public async Task<IActionResult> CreateArtworkAsync([FromForm] CreateArtworkForm createArtworkForm)
        {
            _logger.LogInformation("User {User} is creating a new artwork: {Title}", User.GetUserName(), createArtworkForm.Title);
            var createdArtwork = await _artworkService.CreateArtworkAsync(createArtworkForm, User);
            _logger.LogInformation("Artwork created successfully: {ArtworkId} by user {User}", createdArtwork.Id, User.GetUserName());

            _cacheService.Remove(CacheKeyNames.AllArtworks); // Invalidate the cache for all artworks

            return CreatedAtAction(nameof(GetAll), new { id = createdArtwork.Id }, createdArtwork);
        }

        /// <summary>
        /// Updates an existing artwork (excluding image).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = RoleNames.Artist)] // Only users with the "Artist" role can access this endpoint.
        // PUT /api/artworks/{id}
        public async Task<IActionResult> UpdateArtworkAsync(int id, [FromBody] ArtworkDto dto)
        {
            _logger.LogInformation("User {User} is updating artwork {ArtworkId}", User.GetUserName(), id);
            var updatedArtwork = await _artworkService.UpdateArtworkAsync(id, dto, User);
            _logger.LogInformation("Artwork {ArtworkId} updated successfully by user {User}", id, User.GetUserName());

            _cacheService.Remove(CacheKeyNames.AllArtworks); // Invalidate the cache for all artworks

            return Ok(updatedArtwork);
        }

        /// <summary>
        /// Deletes an artwork and its image.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleNames.Artist)] // Only users with the "Artist" role can access this endpoint.
        // DELETE /api/artworks/{id}
        public async Task<IActionResult> DeleteArtworkAsync(int id)
        {
            _logger.LogInformation("User {User} is deleting artwork {ArtworkId}", User.GetUserName(), id);
            await _artworkService.DeleteArtworkAsync(id, User);
            _logger.LogInformation("Artwork {ArtworkId} deleted successfully by user {User}", id, User.GetUserName());

            _cacheService.Remove(CacheKeyNames.AllArtworks); // Invalidate the cache for all artworks

            return NoContent();
        }

        /// <summary>
        /// Retrieves all artworks from the database.
        /// </summary>
        [HttpGet]
        // GET /api/artworks
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Retrieving all artworks (with caching)");

            var artworks = await _cacheService.GetOrCreateAsync(
                cacheKey: CacheKeyNames.AllArtworks,
                factory: () => _artworkService.GetAllArtworksAsync(),
                absoluteExpireTime: TimeSpan.FromSeconds(60)
            );

            _logger.LogInformation("Returned {Count} artworks", artworks?.Count() ?? 0);
            return Ok(artworks);
        }

        /// <summary>
        /// Retrieves artworks created by the currently authenticated user.
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = RoleNames.Artist)]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, NoStore = false)] // Caches the response for 30 seconds to improve performance.
        // GET /api/artworks/my
        public async Task<IActionResult> GetMyArtworksAsync()
        {
            int userId = User.GetUserId();
            _logger.LogInformation("Retrieving artworks for user {UserId}", userId);
            var artworks = await _artworkService.GetMyArtworksAsync(userId);
            _logger.LogInformation("Returned {Count} artworks for user {UserId}", artworks?.Count() ?? 0, userId);
            return Ok(artworks);
        }
    }
}