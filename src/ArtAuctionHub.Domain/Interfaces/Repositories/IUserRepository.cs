using ArtAuctionHub.Domain.Entities;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract for <see cref="User"/> aggregate operations.
    /// Implementations encapsulate persistence details while exposing
    /// domain-oriented queries that the Application layer can rely on.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Checks whether a user with the provided identifier exists.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if the user exists; otherwise <c>false</c>.</returns>
        Task<bool> ExistsAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a user exists with the specified e-mail address.
        /// </summary>
        /// <param name="email">E-mail address to check.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

        /// <summary>
        /// Determines whether a user exists with the specified username.
        /// </summary>
        /// <param name="username">Username to check.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);

        /// <summary>
        /// Gets a user by e-mail, or <c>null</c> when no user is found.
        /// </summary>
        /// <param name="email">E-mail to search by.</param>
        /// <param name="ct">A token to observe while waiting for the task to complete.</param>
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    }
}