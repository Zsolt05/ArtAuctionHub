using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A implementation of IBidService interface to manage bids on auctions.
    /// </summary>
    public class BidService(ArtAuctionHubDbContext dbContext) : IBidService
    {
        public async Task<List<BidDto>> GetBidsForUserAsync(int userId)
        {
            // Validate the user exists in the database
            if (!await dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }
            // Retrieve bids for the specified user
            var bids = await dbContext.Bids
                .Where(b => b.UserId == userId)
                .Select(b => new BidDto
                {
                    Amount = b.Amount,
                    AuctionId = b.AuctionId,
                })
                .ToListAsync();
            return bids;
        }

        public async Task PlaceBidAsync(BidDto dto, int userId)
        {
            // Validate the user exists in the database
            if (!await dbContext.Users.AnyAsync(u => u.Id == userId))
            {
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }

            var auction = await dbContext.Auctions
                .FirstOrDefaultAsync(a => a.Id == dto.AuctionId) 
                ?? throw new ArgumentException($"Auction with ID {dto.AuctionId} does not exist.");

            if (auction.EndDate < DateTime.UtcNow)
            {
                throw new ArgumentException($"Auction with ID {dto.AuctionId} has already ended.");
            }

            // Get the maximum bid amount for the specified auction
            var maxBid = await dbContext.Bids
                .Where(b => b.AuctionId == dto.AuctionId)
                .MaxAsync(b => (decimal?)b.Amount) ?? 0;
            // Validate the bid amount is greater than the current maximum bid
            if (dto.Amount <= maxBid)
            {
                throw new ArgumentException($"Bid amount must be greater than the current maximum bid of {maxBid}.");
            }
            // Create a new Bid entity and add it to the context
            var bid = new Domain.Entities.Bid
            {
                Amount = dto.Amount,
                AuctionId = dto.AuctionId,
                UserId = userId,
                BidDate = DateTime.UtcNow
            };
            await dbContext.Bids.AddAsync(bid);
            // Save changes to the database
            await dbContext.SaveChangesAsync();
        }
    }
}