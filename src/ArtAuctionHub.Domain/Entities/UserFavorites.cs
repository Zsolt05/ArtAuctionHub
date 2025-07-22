namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a user's favorite artworks in the auction system.
    /// </summary>
    public class UserFavorites
    {
        /// <summary>
        /// Unique identifier for the UserFavorites entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The date and time when the artwork was added to favorites.
        /// </summary>
        public DateTime AddedDate { get; set; } = DateTime.Now;
        /// <summary>
        /// The ID of the user who has favorited artworks.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// The ID of the artwork that is favorited by the user.
        /// </summary>
        public int ArtworkId { get; set; }
        /// <summary>
        /// Navigation property to the User entity.
        /// </summary>
        public User User { get; set; } = default!;
        /// <summary>
        /// Navigation property to the Artwork entity.
        /// </summary>
        public Artwork Artwork { get; set; } = default!;
    }
}