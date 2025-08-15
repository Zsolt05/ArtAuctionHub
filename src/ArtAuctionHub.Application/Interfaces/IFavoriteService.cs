using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Provides the contract for managing user's favorite artworks.
    /// </summary>
    public interface IFavoriteService
    {
        /// <summary>
        /// Adds an artwork to the user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be added to favorites.</param>
        /// <param name="userId">The ID of the user who is adding the artwork to favorites.</param>
        Task AddFavoriteAsync(int artworkId, int userId);

        /// <summary>
        /// Removes an artwork from the user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be removed from favorites.</param>
        /// <param name="userId">The ID of the user who is removing the artwork from favorites.</param>
        Task RemoveFavoriteAsync(int artworkId, int userId);

        /// <summary>
        /// Retrieves a list of the user's favorite artworks.
        /// </summary>
        /// <param name="userId">The ID of the user whose favorites are to be retrieved.</param>
        /// <returns>Collection of artworks that are marked as favorites by the user.</returns>
        Task<List<Artwork>> GetFavoritesAsync(int userId);
    }
}