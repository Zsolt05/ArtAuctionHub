namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a category for artworks in the auction system.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Unique identifier for the category.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the category.
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// Collection of artworks associated with this category.
        /// </summary>
        public ICollection<Artwork> Artworks { get; set; } = [];
    }
}