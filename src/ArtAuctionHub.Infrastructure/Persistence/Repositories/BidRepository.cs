using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using ArtAuctionHub.Shared.Exceptions;
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

        /// <inheritdoc />
        public async Task<Bid> GetHighestBidForAuctionAsync(int auctionId, CancellationToken ct = default)
        {
            // Get the highest bid for the auction
            var highestBid = await _set
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .ThenByDescending(b => b.BidDate)
                .Select(b => new Bid
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    BidDate = b.BidDate,
                    UserId = b.UserId,
                    AuctionId = b.AuctionId
                })
                .FirstOrDefaultAsync(ct);

            if (highestBid != null)
                return highestBid;

            // Only fetch the necessary fields for the auction
            var auctionData = await _db.Auctions
                .Where(a => a.Id == auctionId)
                .Select(a => new { a.Id, a.StartingPrice, a.StartDate, a.Artwork.ArtistId })
                .SingleOrDefaultAsync(ct)
                ?? throw new NotFoundException($"Auction with id {auctionId} not found.");

            // No bids exist, return a starting bid
            return new Bid
            {
                Amount = auctionData.StartingPrice,
                AuctionId = auctionData.Id,
                BidDate = auctionData.StartDate,
                UserId = auctionData.ArtistId
            };
        }
    }
}