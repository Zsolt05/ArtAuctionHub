using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Auction-oriented application service that orchestrates domain repositories.
    /// This service has no EF Core dependency; it relies purely on repository abstractions,
    /// which improves testability and keeps upper layers decoupled from the ORM.
    /// </summary>
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctions;
        private readonly IRepository<Artwork> _artworks;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AuctionService> _logger;

        /// <summary>
        /// Creates a new instance of the service with the required dependencies.
        /// </summary>
        /// <param name="auctions">Repository for <see cref="Auction"/> aggregates.</param>
        /// <param name="artworks">Generic repository for <see cref="Artwork"/> used to validate references.</param>
        /// <param name="uow">Unit of Work coordinating persistence and transactions.</param>
        /// <param name="logger">Logger for auction actions.</param>
        public AuctionService(
            IAuctionRepository auctions,
            IRepository<Artwork> artworks,
            IUnitOfWork uow,
            ILogger<AuctionService> logger)
        {
            _auctions = auctions;
            _artworks = artworks;
            _uow = uow;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Auction> CreateAuctionAsync(int userId, Auction createAuction)
        {
            _logger.LogInformation("User {UserId} is creating an auction for artwork {ArtworkId}", userId, createAuction.ArtworkId);
            var artworkExists = await _artworks.AnyAsync(a => a.Id == createAuction.ArtworkId && a.ArtistId == userId);
            if (!artworkExists)
            {
                _logger.LogWarning("Artwork with ID {ArtworkId} does not exist for user {UserId}", createAuction.ArtworkId, userId);
                throw new ArgumentException($"Artwork with ID {createAuction.ArtworkId} does not exist.");
            }

            await _auctions.AddAsync(createAuction);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Auction created for artwork {ArtworkId} by user {UserId}", createAuction.ArtworkId, userId);

            return createAuction;
        }

        /// <inheritdoc />
        public async Task<Auction> UpdateAuctionAsync(int userId, int auctionId, Auction dto)
        {
            _logger.LogInformation("User {UserId} is updating auction {AuctionId}", userId, auctionId);
            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == auctionId && a.Artwork.ArtistId == userId);
            if (auction is null)
            {
                _logger.LogWarning("Auction with ID {AuctionId} not found for user {UserId}", auctionId, userId);
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }

            auction.StartDate = dto.StartDate;
            auction.EndDate = dto.EndDate;
            auction.StartingPrice = dto.StartingPrice;
            auction.ArtworkId = dto.ArtworkId;

            _auctions.Update(auction);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Auction {AuctionId} updated by user {UserId}", auctionId, userId);

            return dto;
        }

        /// <inheritdoc />
        public async Task DeleteAuctionAsync(int userId, int auctionId)
        {
            _logger.LogInformation("User {UserId} is deleting auction {AuctionId}", userId, auctionId);
            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == auctionId && a.Artwork.ArtistId == userId);
            if (auction is null)
            {
                _logger.LogWarning("Auction with ID {AuctionId} not found for user {UserId}", auctionId, userId);
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }

            _auctions.Remove(auction);
            await _uow.SaveChangesAsync();
            _logger.LogInformation("Auction {AuctionId} deleted by user {UserId}", auctionId, userId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            _logger.LogInformation("Retrieving all active auctions");
            var now = DateTime.UtcNow;
            var items = await _auctions.ListActiveAsync(now);
            _logger.LogInformation("Returned {Count} active auctions", items.Count);
            return items;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Auction>> GetUserAuctionsAsync(int userId)
        {
            _logger.LogInformation("Retrieving auctions for user {UserId}", userId);
            var items = await _auctions.ListForCurrentUserAsync(userId);
            _logger.LogInformation("Returned {Count} auctions for user {UserId}", items.Count, userId);
            return items;
        }

        /// <inheritdoc />
        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving auction details for auction {AuctionId}", id);
            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction is null)
            {
                _logger.LogWarning("Auction with ID {AuctionId} not found", id);
                return null;
            }
            return auction;
        }
    }
}