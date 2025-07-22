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
        void PlaceBid(BidDto dto);

        /// <summary>
        /// Retrieves a list of all bids placed by the currently authenticated user.
        /// </summary>
        /// <returns>A collection of BidDto representing the user's bids.</returns>
        IEnumerable<BidDto> GetMyBids();
    }
}