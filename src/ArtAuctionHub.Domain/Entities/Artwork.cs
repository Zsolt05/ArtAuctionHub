namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents an artwork that can be auctioned.
    /// </summary>
    public class Artwork
    {
        /// <summary>
        /// Unique identifier for the artwork.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Title of the artwork.
        /// </summary>
        public string Title { get; set; } = default!;
        /// <summary>
        /// Description of the artwork.
        /// </summary>
        public string Description { get; set; } = default!;
        /// <summary>
        /// The ID of the Artist who created the artwork.
        /// </summary>
        public int ArtistId { get; set; }
        /// <summary>
        /// The Artist who created the artwork.
        /// </summary>
        public User Artist { get; set; } = default!;
        /// <summary>
        /// The category to which this artwork belongs.
        /// </summary>
        public Category Category { get; set; } = default!;
        /// <summary>
        /// The ID of the category to which this artwork belongs.
        /// </summary>
        public int CategoryId { get; set; }
    }
}