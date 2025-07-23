namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Join entity that represents the many-to-many relationship
    /// between Users and Roles.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The user assigned to this role.
        /// </summary>
        public User User { get; set; } = default!;

        /// <summary>
        /// The ID of the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// The role assigned to the user.
        /// </summary>
        public Role Role { get; set; } = default!;
    }
}