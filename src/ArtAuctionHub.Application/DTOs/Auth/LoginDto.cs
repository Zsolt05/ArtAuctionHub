namespace ArtAuctionHub.Application.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object (DTO) used for login requests.
    /// Contains the user's email and password for authentication.
    /// </summary>
    public record LoginDto
    {
        /// <summary>
        /// The email address of the user attempting to log in.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// The user's password (in plaintext for the request).
        /// In production, this password should be hashed on the server side.
        /// </summary>
        public string Password { get; set; } = default!;
    }
}
