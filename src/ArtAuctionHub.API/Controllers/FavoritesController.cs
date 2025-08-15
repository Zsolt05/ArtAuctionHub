using ArtAuctionHub.Application.DTOs.ArtWork;
using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The FavoritesController manages a user's favorite artworks.
    /// </summary>
    /// <param name="_favoriteService">An instance of IFavoriteService to handle favorite operations. Registered in the Dependency Injection (DI) container.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/favorites")] // Base route for favorites-related endpoints. (/api/favorites)
    public class FavoritesController(IFavoriteService _favoriteService) : ControllerBase
    {
        /// <summary>
        /// Adds an artwork to the current user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to add.</param>
        /// <returns>A 200 OK response with a confirmation message.</returns>
        [HttpPost]
        [Route("{artworkId}")] // Matches POST /api/favorites/{artworkId}.
        public async Task<IActionResult> AddFavorite(int artworkId)
        {
            int userId = 1; // Placeholder for the current user's ID. In a future implementation, this would be retrieved from the authenticated user's context.
            // Future implementation would involve:
            // 1. Validating the artwork ID.
            // 2. Checking if the artwork already exists in user's favorites.
            // 3. Adding the artwork to the user's favorites in the database.
            await _favoriteService.AddFavoriteAsync(artworkId, userId);
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
            int userId = 1; // Placeholder for the current user's ID. In a future implementation, this would be retrieved from the authenticated user's context.
            // Future implementation would involve:
            // 1. Validating the artwork ID.
            // 2. Checking if the artwork exists in user's favorites.
            // 3. Removing the artwork from the user's favorites in the database.
            await _favoriteService.RemoveFavoriteAsync(artworkId, userId);
            return Ok(new { Message = $"Artwork {artworkId} removed from favorites." });
        }

        /// <summary>
        /// Retrieves all favorite artworks of the current user.
        /// </summary>
        /// <returns>A 200 OK response with the list of favorite artworks.</returns>
        [HttpGet] // Matches GET /api/favorites.
        public async Task<IActionResult> GetFavorites()
        {
            int userId = 1; // Placeholder for the current user's ID. In a future implementation, this would be retrieved from the authenticated user's context.

            // Future implementation would involve:
            // 1. Fetching the user's favorite artworks from the database.
            // 2. Returning the list of favorite artworks in a suitable format (e.g., list of ArtworkDto).
            var favorites = await _favoriteService.GetFavoritesAsync(userId);
            var favoritesDto = favorites.Select(f => new ArtworkDto
            {
                Title = f.Title,
                Description = f.Description,
                ImageUrl = f.ImageUrl,
                CategoryId = f.CategoryId,
                CodeName = f.CodeName,
                IsAdultOnly = f.IsAdultOnly
            }).ToList();
            return Ok(favoritesDto); // Returns 200 OK with the list of favorite artworks.
        }
    }
}