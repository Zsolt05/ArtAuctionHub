using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The FavoritesController manages a user's favorite artworks.
    /// </summary>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/favorites")] // Base route for favorites-related endpoints. (/api/favorites)
    public class FavoritesController : ControllerBase
    {
        /// <summary>
        /// Adds an artwork to the current user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to add.</param>
        /// <returns>A 200 OK response with a confirmation message.</returns>
        [HttpPost]
        [Route("{artworkId}")] // Matches POST /api/favorites/{artworkId}.
        public IActionResult AddFavorite(int artworkId)
        {
            return Ok(new { Message = $"Artwork {artworkId} added to favorites." });
        }

        /// <summary>
        /// Removes an artwork from the user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to remove.</param>
        /// <returns>A 200 OK response with a confirmation message.</returns>
        [HttpDelete]
        [Route("{artworkId}")] // Matches DELETE /api/favorites/{artworkId}.
        public IActionResult RemoveFavorite(int artworkId)
        {
            return Ok(new { Message = $"Artwork {artworkId} removed from favorites." });
        }

        /// <summary>
        /// Retrieves all favorite artworks of the current user.
        /// </summary>
        /// <returns>A 200 OK response with the list of favorite artworks.</returns>
        [HttpGet] // Matches GET /api/favorites.
        public IActionResult GetFavorites()
        {
            return Ok(new[] { "Favorite1", "Favorite2" });
        }
    }
}