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
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/artworks")] // Base route for all artwork-related endpoints. (/api/artworks)
    [Authorize]
    public class ArtworksController : ControllerBase
    {
        private readonly IArtworkService _artworkService;
        private readonly ILogger<ArtworksController> _logger;

        /// <summary>
        /// Constructor for ArtworksController.
        /// </summary>
        /// <param name="artworkService">Injected artwork service.</param>
        /// <param name="logger">Logger for artwork actions.</param>
        public ArtworksController(IArtworkService artworkService, ILogger<ArtworksController> logger)
        {
            _artworkService = artworkService;
            _logger = logger;
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
        public async Task<IActionResult> CreateArtworkAsync([FromForm] ArtworkDto dto, [FromForm] IFormFile imageFile)
        {
            _logger.LogInformation("User {User} is creating a new artwork: {Title}", User.GetUserName(), dto.Title);
            var createdArtwork = await _artworkService.CreateArtworkAsync(dto, imageFile, User);
            _logger.LogInformation("Artwork created successfully: {ArtworkId} by user {User}", createdArtwork.Id, User.GetUserName());
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
            return NoContent();
        }

        /// <summary>
        /// Retrieves all artworks from the database.
        /// </summary>
        [HttpGet]
        // GET /api/artworks
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Retrieving all artworks");
            var artworks = await _artworkService.GetAllArtworksAsync();
            _logger.LogInformation("Returned {Count} artworks", artworks?.Count() ?? 0);
            return Ok(artworks);
        }

        /// <summary>
        /// Retrieves artworks created by the currently authenticated user.
        /// </summary>
        [HttpGet("my")]
        [Authorize(Roles = RoleNames.Artist)]
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