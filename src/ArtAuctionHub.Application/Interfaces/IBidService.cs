using ArtAuctionHub.Application.DTOs.Bid;

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
    }
}