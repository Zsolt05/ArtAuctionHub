using Microsoft.AspNetCore.Identity;

namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a user of the Art Auction Hub application.
    /// </summary>
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// The user's birth date.
        /// </summary>
        public DateOnly BirthDate { get; set; }

        /// <summary>
        /// Navigation property for the user's favorite artworks.
        /// </summary>
        public ICollection<UserFavorites> Favorites { get; set; } = [];
        /// <summary>
        /// Navigation property for the artworks created by artist (only for artists).
        /// </summary>
        public ICollection<Artwork> Artworks { get; set; } = [];
        /// <summary>
        /// Navigation property for the UserRoles associated with the user.
        /// </summary>
        public ICollection<UserRole> Roles { get; set; } = [];
    }
}