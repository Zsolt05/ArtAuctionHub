using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract for link operations between <see cref="User"/> and <see cref="Role"/>.
    /// </summary>
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        /// <summary>
        /// Determines whether the user already has the specified role assigned.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="roleId">Role identifier.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<bool> HasRoleAsync(int userId, int roleId, CancellationToken ct = default);
    }
}