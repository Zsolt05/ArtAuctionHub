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
        void AddFavorite(int artworkId);

        /// <summary>
        /// Removes an artwork from the user's favorites.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be removed from favorites.</param>
        void RemoveFavorite(int artworkId);

        /// <summary>
        /// Retrieves a list of the user's favorite artworks.
        /// </summary>
        /// <returns>Collection of artwork IDs that are marked as favorites by the user.</returns>
        IEnumerable<string> GetFavorites();
    }
}