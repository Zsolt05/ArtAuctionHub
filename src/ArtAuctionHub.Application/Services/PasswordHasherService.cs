namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// Provides password hashing and verification using BCrypt (built-in salt).
    /// </summary>
    public class PasswordHasherService
    {
        /// <summary>
        /// Hashes the password using BCrypt with a random salt internally.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <returns>Hashed password (contains salt internally).</returns>
        public static string HashPassword(string password) =>
            // WorkFactor defines the computational cost (12 is a good default)
            BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

        /// <summary>
        /// Verifies a password against the stored BCrypt hash.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <param name="hashedPassword">The previously stored hash.</param>
        /// <returns>True if the password is correct, otherwise false.</returns>
        public static bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
