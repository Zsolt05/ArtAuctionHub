using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository abstraction dedicated to working with <see cref="Bid"/> aggregates.
    /// Use this repository for domain-specific read/write operations that go beyond the
    /// generic CRUD capabilities provided by <see cref="IRepository{T}"/>.
    /// <para>
    /// Typical consumers are Application-layer services and use-case handlers which
    /// should not reference any infrastructure concerns (e.g., EF Core).
    /// </para>
    /// </summary>
    public interface IBidRepository : IRepository<Bid>
    {
        /// <summary>
        /// Returns all bids placed by a specific user.
        /// </summary>
        /// <param name="userId">Identifier of the user.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of bids ordered by bid date descending.</returns>
        Task<List<Bid>> ListByUserAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Returns the maximum bid amount for a given auction or <c>0</c> if there are no bids.
        /// </summary>
        /// <param name="auctionId">Identifier of the auction.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The maximum amount found or <c>0</c> when no bids exist.</returns>
        Task<decimal> GetMaxAmountForAuctionAsync(int auctionId, CancellationToken ct = default);
    }
}