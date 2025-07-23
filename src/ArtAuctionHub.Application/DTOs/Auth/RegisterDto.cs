namespace ArtAuctionHub.Application.DTOs.Auth
{
    /// <summary>
    /// Data Transfer Object (DTO) used when registering a new user.
    /// This contains all necessary information to create a user account.
    /// </summary>
    public record RegisterDto
    {
        /// <summary>
        /// The username chosen by the user.
        /// This is a display name and must be unique.
        /// </summary>
        public string UserName { get; set; } = default!;

        /// <summary>
        /// The user's email address.
        /// This is used for login and must be unique.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// The password chosen by the user.
        /// It should meet password strength requirements.
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// The user's birth date.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The role of the user within the system.
        /// Default value: "Buyer".
        /// Possible values: "Buyer" or "Artist".
        /// </summary>
        public string Role { get; set; } = "Buyer"; // Default role is Buyer.
    }
}
