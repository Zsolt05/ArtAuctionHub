using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract dedicated to <see cref="Artwork"/> aggregates.
    /// Inherits the generic CRUD/query surface from <c>IRepository&lt;Artwork&gt;</c>
    /// and adds domain-specific queries that are frequently used by the application.
    /// <para>
    /// Keep DTO mapping outside the repository; repositories should return domain entities.
    /// </para>
    /// </summary>
    public interface IArtworkRepository : IRepository<Artwork>
    {
        /// <summary>
        /// Retrieves all artworks as a non-tracked list. Use this for read-only scenarios.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>All artworks from the store with change tracking disabled.</returns>
        Task<List<Artwork>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves artworks that belong to a specific artist.
        /// </summary>
        /// <param name="artistId">The identifier of the artist.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Artworks created by the specified artist.</returns>
        Task<List<Artwork>> GetByArtistIdAsync(int artistId, CancellationToken ct = default);

        /// <summary>
        /// Attempts to find a single artwork by its primary key.
        /// </summary>
        /// <param name="id">Artwork identifier.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The artwork if found; otherwise <c>null</c>.</returns>
        Task<Artwork?> FindByIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Checks whether an artwork with the provided identifier exists.
        /// </summary>
        /// <param name="artworkId">The artwork identifier.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if the artwork exists; otherwise <c>false</c>.</returns>
        Task<bool> ExistsAsync(int artworkId, CancellationToken ct = default);
    }
}