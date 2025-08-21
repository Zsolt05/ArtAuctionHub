using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="IBidRepository"/>.
    /// This class is internal to the Infrastructure layer and should be resolved
    /// through its interface via DI.
    /// </summary>
    internal sealed class BidRepository : EfRepository<Bid>, IBidRepository
    {
        /// <summary>
        /// Initializes a new repository using the shared <see cref="ArtAuctionHubDbContext"/>.
        /// </summary>
        /// <param name="db">The application's EF Core database context.</param>
        public BidRepository(ArtAuctionHubDbContext db) : base(db) { }

        /// <inheritdoc />
        public async Task<List<Bid>> ListByUserAsync(int userId, CancellationToken ct = default)
        {
            return await _set
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BidDate)
                .ToListAsync(ct);
        }

        /// <inheritdoc />
        public async Task<decimal> GetMaxAmountForAuctionAsync(int auctionId, CancellationToken ct = default)
        {
            var max = await _set
                .Where(b => b.AuctionId == auctionId)
                .MaxAsync(b => (decimal?)b.Amount, ct);

            return max ?? 0m;
        }
    }
}