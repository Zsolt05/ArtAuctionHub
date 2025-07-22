namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents an auction for an artwork.
    /// </summary>
    public class Auction
    {
        /// <summary>
        /// Unique identifier for the auction.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The starting price of the auction.
        /// </summary>
        public decimal StartingPrice { get; set; }
        /// <summary>
        /// The date and time when the auction starts.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// The date and time when the auction ends.
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// The ID of the artwork being auctioned.
        /// </summary>
        public int ArtworkId { get; set; }
        /// <summary>
        /// The artwork being auctioned.
        /// </summary>
        public Artwork Artwork { get; set; } = default!;
        /// <summary>
        /// A collection of bids placed on this auction.
        /// </summary>
        public ICollection<Bid> Bids { get; set; } = [];
    }
}