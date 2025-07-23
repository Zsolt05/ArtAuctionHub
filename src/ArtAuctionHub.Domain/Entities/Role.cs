namespace ArtAuctionHub.Domain.Entities
{
    /// <summary>
    /// Represents a role in the auction system, such as admin or user.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Unique identifier for the Role entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name of the role (e.g., "Buyer","Artist").
        /// </summary>
        public string Name { get; set; } = default!;
        /// <summary>
        /// A collection of UserRole entities that represent the many-to-many relationship
        /// </summary>
        public ICollection<UserRole> Users { get; set; } = [];
    }
}
