namespace ArtAuctionHub.Application.DTOs.Bid
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a bid placed on an auction.
    /// </summary>
    public record BidDto
    {
        /// <summary>
        /// The ID of the auction on which the bid is placed.
        /// </summary>
        public int AuctionId { get; set; }

        /// <summary>
        /// The amount of the bid (monetary value).
        /// </summary>
        public decimal Amount { get; set; }
    }
}
