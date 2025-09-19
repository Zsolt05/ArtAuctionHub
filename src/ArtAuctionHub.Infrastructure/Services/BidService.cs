using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using ArtAuctionHub.Shared.Exceptions;
using Microsoft.Extensions.Logging;

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
        private readonly IAuctionRepository _auctions;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<BidService> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="BidService"/>.
        /// </summary>
        /// <param name="bids">Repository for <see cref="Bid"/> aggregates.</param>
        /// <param name="users">Generic repository for <see cref="User"/> checks.</param>
        /// <param name="auctions">Repository for <see cref="Auction"/> aggregates.</param>
        /// <param name="uow">Unit of work coordinating the current persistence context.</param>
        /// <param name="logger">Logger for bid actions.</param>
        public BidService(
            IBidRepository bids,
            IRepository<User> users,
            IAuctionRepository auctions,
            IUnitOfWork uow,
            ILogger<BidService> logger)
        {
            _bids = bids;
            _users = users;
            _auctions = auctions;
            _uow = uow;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all bids placed by the specified user and projects them into <see cref="BidDto"/>.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of bid DTOs for the user.</returns>
        /// <exception cref="ArgumentException">Thrown when the user does not exist.</exception>
        public async Task<List<BidDto>> GetBidsForUserAsync(int userId)
        {
            _logger.LogInformation("Retrieving bids for user {UserId}", userId);
            if (!await _users.AnyAsync(u => u.Id == userId))
            {
                _logger.LogWarning("User with ID {UserId} does not exist.", userId);
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }

            var bids = await _bids.ListByUserAsync(userId);
            _logger.LogInformation("Returned {Count} bids for user {UserId}", bids.Count, userId);

            return bids.Select(b => new BidDto
            {
                Amount = b.Amount,
                AuctionId = b.AuctionId
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<Bid> GetHighestBidForAuctionAsync(int auctionId)
        {
            if (!await _auctions.AnyAsync(a => a.Id == auctionId))
            {
                _logger.LogWarning("Auction with ID {AuctionId} does not exist.", auctionId);
                throw new NotFoundException("Auction not found with ID " + auctionId);
            }

            if (!await _auctions.IsActiveAsync(auctionId, DateTime.UtcNow))
            {
                _logger.LogWarning("Auction with ID {AuctionId} is not active.", auctionId);
                throw new NotFoundException($"Auction with ID {auctionId} is not active.");
            }

            _logger.LogInformation("Retrieving highest bid for auction {AuctionId}", auctionId);
            return await _bids.GetHighestBidForAuctionAsync(auctionId);
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
            _logger.LogInformation("User {UserId} is placing a bid of {Amount} on auction {AuctionId}", userId, dto.Amount, dto.AuctionId);
            if (!await _users.AnyAsync(u => u.Id == userId))
            {
                _logger.LogWarning("User with ID {UserId} does not exist.", userId);
                throw new ArgumentException($"User with ID {userId} does not exist.");
            }

            if (!await _auctions.IsActiveAsync(dto.AuctionId, DateTime.UtcNow))
            {
                _logger.LogWarning("Auction with ID {AuctionId} is not active.", dto.AuctionId);
                throw new ArgumentException($"Auction with ID {dto.AuctionId} is not active or does not exist.");
            }

            var maxBid = await _bids.GetMaxAmountForAuctionAsync(dto.AuctionId);
            if (dto.Amount <= maxBid)
            {
                _logger.LogWarning("Bid amount {Amount} is not greater than current max bid {MaxBid} for auction {AuctionId}", dto.Amount, maxBid, dto.AuctionId);
                throw new ArgumentException($"Bid amount must be greater than the current maximum bid of {maxBid}.");
            }

            var bid = new Bid
            {
                Amount = dto.Amount,
                AuctionId = dto.AuctionId,
                UserId = userId,
                BidDate = DateTime.UtcNow
            };

            await _bids.AddAsync(bid);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Bid of {Amount} placed successfully by user {UserId} on auction {AuctionId}", dto.Amount, userId, dto.AuctionId);
        }
    }
}