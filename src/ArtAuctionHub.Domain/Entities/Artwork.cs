using System.ComponentModel.DataAnnotations.Schema;

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
        /// A flag indicating if the artwork is restricted to adult audiences (18+ content).
        /// </summary>
        public bool IsAdultOnly { get; set; } = true;
        /// <summary>
        /// The location of the artwork image.
        /// </summary>
        public string ImageUrl { get; set; } = default!;
        /// <summary>
        /// The date and time when the artwork was created or added to the system.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// The ID of the Artist who created the artwork.
        /// </summary>
        public int ArtistId { get; set; }
        /// <summary>
        /// The name of the Artist who created the artwork.
        /// </summary>
        public string ArtistName { get; set; } = default!;
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

        [NotMapped]
        public string CodeName => $"{ArtistName.Replace(" ", "_")}-{Title.Replace(" ", "_")}";
    }
}