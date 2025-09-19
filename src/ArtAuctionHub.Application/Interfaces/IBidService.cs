using ArtAuctionHub.Application.DTOs.Bid;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Shared.Exceptions;

namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Provides the contract for placing and retrieving bids
    /// in the auction system.
    /// </summary>
    public interface IBidService
    {
        /// <summary>
        /// Places a bid on an auction item.
        /// </summary>
        /// <param name="dto">Bid data transfer object containing the bid details.</param>
        /// <param name="userId">The ID of the user placing the bid.</param>
        Task PlaceBidAsync(BidDto dto, int userId);

        /// <summary>
        /// Retrieves a list of all bids placed by a specific user.
        /// </summary>
        /// <returns>A collection of BidDto representing the user's bids.</returns>
        Task<List<BidDto>> GetBidsForUserAsync(int userId);

        /// <summary>
        /// Retrieves the highest bid for a specific auction.
        /// </summary>
        /// <remarks>
        /// If no bids have been placed, returns the auction's starting price as a Bid with Amount set to the starting price.
        /// </remarks>
        /// <param name="auctionId">Auction identifier.</param>
        /// <exception cref="NotFoundException">Thrown when the auction does not exist.</exception>"
        /// <returns>The highest Bid for the specified auction.</returns>
        Task<Bid> GetHighestBidForAuctionAsync(int auctionId);
    }
}