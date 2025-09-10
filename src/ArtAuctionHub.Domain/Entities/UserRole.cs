using Microsoft.AspNetCore.Identity;

namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Join entity that represents the many-to-many relationship
    /// between Users and Roles.
    /// </summary>
    public class UserRole : IdentityUserRole<int>
    {
        /// <summary>
        /// The user assigned to this role.
        /// </summary>
        public User User { get; set; } = default!;

        /// <summary>
        /// The role assigned to the user.
        /// </summary>
        public Role Role { get; set; } = default!;
    }
}