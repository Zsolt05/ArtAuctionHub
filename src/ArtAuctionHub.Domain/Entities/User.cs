namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a user of the Art Auction Hub application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Username of the user.
        /// </summary>
        public string Username { get; set; } = default!;
        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = default!;
        /// <summary>
        /// Password hash for the user's password.
        /// </summary>
        public string PasswordHash { get; set; } = default!;
        /// <summary>
        /// Navigation property for the user's favorite artworks.
        /// </summary>
        public ICollection<UserFavorites> Favorites { get; set; } = [];
        /// <summary>
        /// Navigation property for the artworks created by artist (only for artists).
        /// </summary>
        public ICollection<Artwork> Artworks { get; set; } = [];
        /// <summary>
        /// Navigation property for the roles assigned to the user.
        /// </summary>
        public ICollection<Role> Roles { get; set; } = [];
    }
}