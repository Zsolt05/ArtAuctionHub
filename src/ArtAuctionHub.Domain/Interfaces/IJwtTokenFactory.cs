namespace ArtAuctionHub.Domain.Interfaces
{
    /// <summary>
    /// Defines a factory for creating JSON Web Tokens (JWT) for authenticated users.
    /// </summary>
    public interface IJwtTokenFactory
    {
        /// <summary>
        /// Creates a signed JWT for the specified user with the given claims.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="username">The username of the authenticated user.</param>
        /// <param name="email">The email address of the authenticated user.</param>
        /// <param name="roles">The collection of roles assigned to the user.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item><description><c>Token</c> – the serialized JWT as a string.</description></item>
        ///   <item><description><c>minutes</c> – the configured token lifetime in minutes.</description></item>
        /// </list>
        /// </returns>
        (string Token, DateTime Expires) CreateToken(int userId, string username, string email, IEnumerable<string> roles);
    }
}