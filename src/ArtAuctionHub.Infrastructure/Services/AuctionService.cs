using ArtAuctionHub.Application.DTOs.Auction;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;

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

        /// <summary>
        /// Creates a new instance of the service with the required dependencies.
        /// </summary>
        /// <param name="auctions">Repository for <see cref="Auction"/> aggregates.</param>
        /// <param name="artworks">Generic repository for <see cref="Artwork"/> used to validate references.</param>
        /// <param name="uow">Unit of Work coordinating persistence and transactions.</param>
        public AuctionService(
            IAuctionRepository auctions,
            IRepository<Artwork> artworks,
            IUnitOfWork uow)
        {
            _auctions = auctions;
            _artworks = artworks;
            _uow = uow;
        }

        /// <summary>
        /// Creates a new auction if the referenced artwork exists.
        /// </summary>
        /// <param name="dto">Input DTO carrying auction details.</param>
        /// <returns>The same DTO or an enriched DTO after persistence.</returns>
        /// <exception cref="ArgumentException">Thrown when the artwork cannot be found.</exception>
        public async Task<AuctionDto> CreateAuctionAsync(AuctionDto dto)
        {
            var artworkExists = await _artworks.AnyAsync(a => a.Id == dto.ArtworkId);
            if (!artworkExists)
                throw new ArgumentException($"Artwork with ID {dto.ArtworkId} does not exist.");

            var auction = new Auction
            {
                ArtworkId = dto.ArtworkId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                StartingPrice = dto.StartingPrice
            };

            await _auctions.AddAsync(auction);
            await _uow.SaveChangesAsync();

            return dto;
        }

        /// <summary>
        /// Updates an existing auction if found; throws when the entity does not exist.
        /// </summary>
        /// <param name="id">Auction identifier.</param>
        /// <param name="dto">Updated values.</param>
        /// <returns>The updated DTO.</returns>
        /// <exception cref="KeyNotFoundException">When the auction cannot be located.</exception>
        public async Task<AuctionDto> UpdateAuctionAsync(int id, AuctionDto dto)
        {
            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction is null)
                throw new KeyNotFoundException($"Auction with ID {id} not found.");

            auction.StartDate = dto.StartDate;
            auction.EndDate = dto.EndDate;
            auction.StartingPrice = dto.StartingPrice;
            auction.ArtworkId = dto.ArtworkId;

            _auctions.Update(auction);
            await _uow.SaveChangesAsync();

            return dto;
        }

        /// <summary>
        /// Deletes an auction by its identifier if it exists.
        /// </summary>
        /// <param name="id">Auction identifier.</param>
        /// <exception cref="KeyNotFoundException">When the auction cannot be located.</exception>
        public async Task DeleteAuctionAsync(int id)
        {
            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction is null)
                throw new KeyNotFoundException($"Auction with ID {id} not found.");

            _auctions.Remove(auction);
            await _uow.SaveChangesAsync();
        }

        /// <summary>
        /// Returns all currently active auctions based on <see cref="DateTime.UtcNow"/>.
        /// </summary>
        public async Task<IEnumerable<AuctionDto>> GetActiveAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            var items = await _auctions.ListActiveAsync(now);

            return items.Select(a => new AuctionDto
            {
                ArtworkId = a.ArtworkId,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                StartingPrice = a.StartingPrice
            });
        }

        /// <summary>
        /// Returns auctions created/owned by the current user. If ownership is not modeled,
        /// returns all auctions. The behavior is delegated to the repository.
        /// </summary>
        public async Task<IEnumerable<AuctionDto>> GetUserAuctionsAsync()
        {
            var items = await _auctions.ListForCurrentUserAsync();

            return items.Select(a => new AuctionDto
            {
                ArtworkId = a.ArtworkId,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                StartingPrice = a.StartingPrice
            });
        }

        /// <summary>
        /// Retrieves a single auction by its identifier.
        /// </summary>
        /// <param name="id">Auction identifier.</param>
        /// <returns>
        /// A populated <see cref="AuctionDto"/> when found; otherwise <c>null</c>.
        /// </returns>
        public async Task<AuctionDto?> GetAuctionByIdAsync(int id)
        {
            var auction = await _auctions.FirstOrDefaultAsync(a => a.Id == id);
            if (auction is null) return null;

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