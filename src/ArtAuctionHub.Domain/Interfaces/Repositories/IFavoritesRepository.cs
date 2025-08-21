using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract for managing a user's favorite artworks.
    /// Exposes domain-specific queries in addition to the generic <see cref="IRepository{T}"/> APIs.
    /// </summary>
    public interface IFavoritesRepository : IRepository<UserFavorites>
    {
        /// <summary>
        /// Gets the favorite entry for a given user and artwork, or <c>null</c> if not found.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="artworkId">The artwork identifier.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<UserFavorites?> GetByUserAndArtworkAsync(int userId, int artworkId, CancellationToken ct = default);

        /// <summary>
        /// Returns the list of <see cref="Artwork"/> entries that the user has marked as favorite.
        /// The result is intended for read-only use.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<List<Artwork>> GetFavoriteArtworksAsync(int userId, CancellationToken ct = default);
    }
}