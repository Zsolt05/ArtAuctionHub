// ArtAuctionHub.Infrastructure/Services/BidService.cs
using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Application-facing service for managing bids on auctions.
    /// This implementation is persistence-agnostic and relies solely on repository
    /// abstractions and a unit of work to coordinate changes.
    /// </summary>
    public class BidService : IBidService
    {
        private readonly IBidRepository _bids;
        private readonly IRepository<User> _users;
        private readonly IRepository<Auction> _auctions;
        private readonly IUnitOfWork _uow;

        /// <summary>
        /// Creates a new instance of <see cref="BidService"/>.
        /// </summary>
        /// <param name="bids">Repository for <see cref="Bid"/> aggregates.</param>
        /// <param name="users">Generic repository for <see cref="User"/> checks.</param>
        /// <param name="auctions">Generic repository for <see cref="Auction"/> checks.</param>
        /// <param name="uow">Unit of work coordinating the current persistence context.</param>
        public BidService(
            IBidRepository bids,
            IRepository<User> users,
            IRepository<Auction> auctions,
            IUnitOfWork uow)
        {
            _bids = bids;
            _users = users;
            _auctions = auctions;
            _uow = uow;
        }

        /// <summary>
        /// Retrieves all bids placed by the specified user and projects them into <see cref="BidDto"/>.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of bid DTOs for the user.</returns>
        /// <exception cref="ArgumentException">Thrown when the user does not exist.</exception>
        public async Task<List<BidDto>> GetBidsForUserAsync(int userId)
        {
            // Validate user existence
            if (!await _users.AnyAsync(u => u.Id == userId))
                throw new ArgumentException($"User with ID {userId} does not exist.");

            var bids = await _bids.ListByUserAsync(userId);

            // simple projection to DTO
            return bids.Select(b => new BidDto
            {
                Amount = b.Amount,
                AuctionId = b.AuctionId
            }).ToList();
        }

        /// <summary>
        /// Places a bid for a given auction on behalf of the specified user.
        /// Performs validation against auction existence, end date and current max bid.
        /// </summary>
        /// <param name="dto">The bid data transfer object (amount and auction id).</param>
        /// <param name="userId">The user's identifier.</param>
        /// <exception cref="ArgumentException">Thrown when the user or auction does not exist, 
        /// when the auction already ended, or when the bid amount is not high enough.</exception>
        public async Task PlaceBidAsync(BidDto dto, int userId)
        {
            // Validate user & auction existence
            if (!await _users.AnyAsync(u => u.Id == userId))
                throw new ArgumentException($"User with ID {userId} does not exist.");

            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == dto.AuctionId)
                          ?? throw new ArgumentException($"Auction with ID {dto.AuctionId} does not exist.");

            if (auction.EndDate < DateTime.UtcNow)
                throw new ArgumentException($"Auction with ID {dto.AuctionId} has already ended.");

            // Business rule: must exceed max bid
            var maxBid = await _bids.GetMaxAmountForAuctionAsync(dto.AuctionId);
            if (dto.Amount <= maxBid)
                throw new ArgumentException($"Bid amount must be greater than the current maximum bid of {maxBid}.");

            // Create and persist new bid
            var bid = new Bid
            {
                Amount = dto.Amount,
                AuctionId = dto.AuctionId,
                UserId = userId,
                BidDate = DateTime.UtcNow
            };

            await _bids.AddAsync(bid);
            await _uow.SaveChangesAsync(); // single UoW commit
        }
    }
}