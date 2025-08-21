using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="Role"/> aggregate operations.
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>
        /// Gets a role by its unique name, or <c>null</c> if not found.
        /// </summary>
        /// <param name="name">Role name (e.g., "Buyer").</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}