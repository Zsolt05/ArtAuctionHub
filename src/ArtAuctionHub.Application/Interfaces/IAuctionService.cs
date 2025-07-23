using ArtAuctionHub.Application.DTOs.Auction;

namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Provides the contract for auction-related business logic,
    /// such as creating, editing, deleting, and retrieving auctions.
    /// </summary>
    public interface IAuctionService
    {
        /// <summary>
        /// Creates a new auction with the provided details.
        /// </summary>
        /// <param name="dto">Auction data transfer object containing the auction details.</param>
        /// <returns>AuctionDto representing the created auction.</returns>
        Task<AuctionDto> CreateAuctionAsync(AuctionDto dto);

        /// <summary>
        /// Updates an existing auction identified by its ID with the provided details.
        /// </summary>
        /// <param name="id">Auction ID to identify the auction to be updated.</param>
        /// <param name="dto">Auction data transfer object containing the updated auction details.</param>
        /// <returns>AuctionDto representing the updated auction.</returns>
        Task<AuctionDto> UpdateAuctionAsync(int id, AuctionDto dto);

        /// <summary>
        /// Deletes an auction identified by its ID.
        /// </summary>
        /// <param name="id">Auction ID to identify the auction to be deleted.</param>
        Task DeleteAuctionAsync(int id);

        /// <summary>
        /// Retrieves a list of active auctions.
        /// </summary>
        /// <returns>A collection of AuctionDto representing active auctions.</returns>
        Task<IEnumerable<AuctionDto>> GetActiveAuctionsAsync();

        /// <summary>
        /// Retrieves a list of auctions created by the currently authenticated user.
        /// </summary>
        /// <returns>A collection of AuctionDto representing the user's auctions.</returns>
        Task<IEnumerable<AuctionDto>> GetUserAuctionsAsync();

        /// <summary>
        /// Retrieves an auction by its ID.
        /// </summary>
        /// <param name="id">Auction ID to identify the auction to be retrieved.</param>
        /// <returns>AuctionDto representing the auction with the specified ID.</returns>
        Task<AuctionDto?> GetAuctionByIdAsync(int id);
    }
}