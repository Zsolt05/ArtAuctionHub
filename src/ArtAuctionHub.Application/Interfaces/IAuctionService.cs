using ArtAuctionHub.Domain.Entities;

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
        /// <param name="userId">User ID of the currently authenticated user.</param>
        /// <param name="createAuction">Auction model containing the auction details.</param>
        /// <returns>AuctionDto representing the created auction.</returns>
        Task<Auction> CreateAuctionAsync(int userId, Auction createAuction);

        /// <summary>
        /// Updates an existing auction identified by its ID with the provided details.
        /// </summary>
        /// <param name="userId">User ID of the currently authenticated user.</param>
        /// <param name="auctionId">Auction ID to identify the auction to be updated.</param>
        /// <param name="updateAuction">Auction model containing the updated auction details.</param>
        /// <returns>AuctionDto representing the updated auction.</returns>
        Task<Auction> UpdateAuctionAsync(int userId, int auctionId, Auction updateAuction);

        /// <summary>
        /// Deletes an auction identified by its ID.
        /// </summary>
        /// <param name="userId">User ID of the currently authenticated user.</param>
        /// <param name="auctionId">Auction ID to identify the auction to be deleted.</param>
        Task DeleteAuctionAsync(int userId, int auctionId);

        /// <summary>
        /// Retrieves a list of active auctions.
        /// </summary>
        /// <returns>A collection of Auctions representing the active auctions.</returns>
        Task<IEnumerable<Auction>> GetActiveAuctionsAsync();

        /// <summary>
        /// Retrieves a list of auctions created by the currently authenticated user.
        /// </summary>
        /// <param name="userId">User ID of the currently authenticated user.</param>
        /// <returns>A collection of Auctions created by the user.</returns>
        Task<IEnumerable<Auction>> GetUserAuctionsAsync(int userId);

        /// <summary>
        /// Retrieves an auction by its ID.
        /// </summary>
        /// <param name="id">Auction ID to identify the auction to be retrieved.</param>
        /// <returns>Auction if found; otherwise, null.</returns>
        Task<Auction?> GetAuctionByIdAsync(int id);
    }
}