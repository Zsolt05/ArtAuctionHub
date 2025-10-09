namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a bid placed by a user on an auction.
    /// </summary>
    public class Bid
    {
        /// <summary>
        /// Unique identifier for the bid.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The amount of the bid.
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The date and time when the bid was placed.
        /// </summary>
        public DateTime BidDate { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// The ID of the user who placed the bid.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The user who placed the bid.
        /// </summary>
        public User User { get; set; } = default!;
        /// <summary>
        /// The ID of the auction on which the bid was placed.
        /// </summary>
        public int AuctionId { get; set; }
        /// <summary>
        /// The auction on which the bid was placed.
        /// </summary>
        public Auction Auction { get; set; } = default!;
    }
}