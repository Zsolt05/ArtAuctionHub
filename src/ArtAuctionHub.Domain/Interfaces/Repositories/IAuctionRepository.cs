using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository abstraction dedicated to the <see cref="Auction"/> aggregate.
    /// Use this interface from upper layers (Application, Services) to encapsulate
    /// auction-specific data access, complex queries and include graphs.
    /// <para>
    /// Keep this interface framework-agnostic (no EF Core types here).
    /// </para>
    /// </summary>
    public interface IAuctionRepository : IRepository<Auction>
    {
        /// <summary>
        /// Retrieves auctions that are currently active at <paramref name="utcNow"/>.
        /// An auction is considered active if <c>StartDate &lt;= utcNow &amp;&amp; EndDate &gt;= utcNow</c>.
        /// </summary>
        /// <param name="utcNow">The timestamp used to determine activity. Expected in UTC.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of active <see cref="Auction"/> entities.</returns>
        Task<List<Auction>> ListActiveAsync(DateTime utcNow, CancellationToken ct = default);

        /// <summary>
        /// Retrieves all auctions "owned by" the current user or scope. If ownership is not
        /// modeled yet, this can temporarily return all auctions. Kept here to make the intent explicit.
        /// </summary>
        /// <param name="userId">The ID of the current user.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of <see cref="Auction"/> entities.</returns>
        Task<List<Auction>> ListForCurrentUserAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Determines if the auction with the given ID is currently active at <paramref name="utcNow"/>.
        /// </summary>
        /// <param name="auctionId">Auction identifier.</param>
        /// <param name="utcNow">A The timestamp used to determine activity. Expected in UTC.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns><see langword="true"/> if the auction is active; otherwise, <see langword="false"/>.</returns>
        Task<bool> IsActiveAsync(int auctionId, DateTime utcNow, CancellationToken ct = default);

        /// <summary>
        /// Retrieves an active auction by its ID if it is currently active at <paramref name="utcNow"/>.
        /// </summary>
        /// <param name="auctionId">Auction identifier.</param>
        /// <param name="utcNow">A The timestamp used to determine activity. Expected in UTC.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The <see cref="Auction"/> if found and active</returns>
        Task<Auction> GetActiveAuctionByIdAsync(int auctionId, DateTime utcNow, CancellationToken ct = default);
    }
}