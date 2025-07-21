namespace ArtAuctionHub.Application.DTOs.Auction
{
    /// <summary>
    /// Data Transfer Object (DTO) representing the data of an auction.
    /// </summary>
    public class AuctionDto
    {
        /// <summary>
        /// The ID of the artwork being auctioned.
        /// </summary>
        public int ArtworkId { get; set; }

        /// <summary>
        /// The start date and time of the auction (UTC).
        /// Example: 2025-07-21T12:00:00Z.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date and time of the auction (UTC).
        /// Example: 2025-07-28T12:00:00Z.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}