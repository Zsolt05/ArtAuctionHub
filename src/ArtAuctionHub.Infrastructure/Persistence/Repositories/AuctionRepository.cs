using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="IAuctionRepository"/>.
    /// Leverages the shared <see cref="ArtAuctionHubDbContext"/> so that the repository
    /// participates in the same Unit of Work/transaction as other repositories resolved
    /// from the same scope.
    /// </summary>
    internal sealed class AuctionRepository
        : EfRepository<Auction>, IAuctionRepository
    {
        /// <summary>
        /// Initializes a new instance of the repository bound to the given DbContext.
        /// </summary>
        /// <param name="db">The EF Core DbContext instance.</param>
        public AuctionRepository(ArtAuctionHubDbContext db) : base(db) { }

        /// <inheritdoc />
        public Task<List<Auction>> ListActiveAsync(DateTime utcNow, CancellationToken ct = default)
            => _set
                .Where(a => a.StartDate <= utcNow && a.EndDate >= utcNow)
                .ToListAsync(ct);

        /// <inheritdoc />
        public Task<List<Auction>> ListForCurrentUserAsync(CancellationToken ct = default)
            // TODO: when ownership is introduced (e.g., a.UserId == currentUserId)
            => _set.ToListAsync(ct);
    }
}