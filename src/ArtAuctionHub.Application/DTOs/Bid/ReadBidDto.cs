namespace ArtAuctionHub.Application.DTOs.Bid
{
    public sealed record ReadBidDto : BidDto
    {
        /// <summary>
        /// The name of the user who placed the bid.
        /// </summary>
        public string UserName { get; init; } = default!;

        /// <summary>
        /// The date and time when the bid was placed.
        /// </summary>
        public DateTime BidDate { get; init; }
    }
}