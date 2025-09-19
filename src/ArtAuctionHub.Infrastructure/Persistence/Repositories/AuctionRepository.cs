using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using ArtAuctionHub.Shared.Exceptions;
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
        public async Task<List<Auction>> ListActiveAsync(DateTime utcNow, CancellationToken ct = default)
            => await _set
                .Where(a => a.StartDate <= utcNow && a.EndDate >= utcNow)
                .ToListAsync(ct);

        /// <inheritdoc />
        public async Task<List<Auction>> ListForCurrentUserAsync(int userId, CancellationToken ct = default)
            => await _set.Where(a => a.Artwork.ArtistId == userId).ToListAsync(ct);

        /// <inheritdoc />
        public async Task<bool> IsActiveAsync(int auctionId, DateTime utcNow, CancellationToken ct = default)
            => await _set.AnyAsync(a => a.Id == auctionId
                                    && a.StartDate <= utcNow
                                    && a.EndDate >= utcNow, ct);

        /// <inheritdoc />
        public async Task<Auction> GetActiveAuctionByIdAsync(int auctionId, DateTime utcNow, CancellationToken ct = default)
        {
            var auction = await _set
                .Include(a => a.Artwork)
                .ThenInclude(art => art.Category)
                .Include(a => a.Bids)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.Id == auctionId
                                       && a.StartDate <= utcNow
                                       && a.EndDate >= utcNow, ct)
               ?? throw new NotFoundException($"Active auction not found with ID {auctionId}");

            return auction;
        }
    }
}