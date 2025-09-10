using Microsoft.AspNetCore.Identity;

namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a role in the auction system, such as admin or user.
    /// </summary>
    public class Role : IdentityRole<int>
    {
        /// <summary>
        /// A collection of UserRole entities that represent the many-to-many relationship
        /// </summary>
        public ICollection<UserRole> Users { get; set; } = [];
    }
}