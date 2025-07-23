using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Auction service implementation using EF Core with async operations.
    /// </summary>
    public class AuctionService : IAuctionService
    {
        private readonly ArtAuctionHubDbContext _dbContext;

        public AuctionService(ArtAuctionHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new auction if the related artwork exists.
        /// </summary>
        public async Task<AuctionDto> CreateAuctionAsync(AuctionDto dto)
        {
            var artworkExists = await _dbContext.Artworks.AnyAsync(a => a.Id == dto.ArtworkId);
            if (!artworkExists)
            {
                throw new ArgumentException($"Artwork with ID {dto.ArtworkId} does not exist.");
            }

            var auction = new Auction
            {
                ArtworkId = dto.ArtworkId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                StartingPrice = dto.StartingPrice
            };

            _dbContext.Auctions.Add(auction);
            await _dbContext.SaveChangesAsync();

            return dto;
        }

        /// <summary>
        /// Updates an existing auction if it exists.
        /// </summary>
        public async Task<AuctionDto> UpdateAuctionAsync(int id, AuctionDto dto)
        {
            var auction = await _dbContext.Auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {id} not found.");
            }

            auction.StartDate = dto.StartDate;
            auction.EndDate = dto.EndDate;
            auction.StartingPrice = dto.StartingPrice;
            auction.ArtworkId = dto.ArtworkId;

            await _dbContext.SaveChangesAsync();
            return dto;
        }

        /// <summary>
        /// Deletes an auction by ID if it exists.
        /// </summary>
        public async Task DeleteAuctionAsync(int id)
        {
            var auction = await _dbContext.Auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {id} not found.");
            }

            _dbContext.Auctions.Remove(auction);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Returns all currently active auctions.
        /// </summary>
        public async Task<IEnumerable<AuctionDto>> GetActiveAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbContext.Auctions
                .Where(a => a.StartDate <= now && a.EndDate >= now)
                .Select(a => new AuctionDto
                {
                    ArtworkId = a.ArtworkId,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    StartingPrice = a.StartingPrice
                })
                .ToListAsync();
        }

        /// <summary>
        /// Returns auctions created by a user (for now returns all auctions).
        /// </summary>
        public async Task<IEnumerable<AuctionDto>> GetUserAuctionsAsync()
        {
            return await _dbContext.Auctions
                .Select(a => new AuctionDto
                {
                    ArtworkId = a.ArtworkId,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    StartingPrice = a.StartingPrice
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a single auction by its ID.
        /// </summary>
        public async Task<AuctionDto?> GetAuctionByIdAsync(int id)
        {
            var auction = await _dbContext.Auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction == null) return null;

            return new AuctionDto
            {
                ArtworkId = auction.ArtworkId,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,
                StartingPrice = auction.StartingPrice
            };
        }
    }
}