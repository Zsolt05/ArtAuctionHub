namespace ArtAuctionHub.Application.DTOs.Auth
{
    /// <summary>
    /// Represents details of the currently authenticated user.
    /// </summary>
    public sealed class CurrentUserDto
    {
        /// <summary>
        /// The username of the authenticated user.
        /// </summary>
        public string Username { get; init; } = default!;

        /// <summary>
        /// The email address of the authenticated user.
        /// </summary>
        public string Email { get; init; } = default!;

        /// <summary>
        /// The birth date of the authenticated user.
        /// </summary>
        public DateOnly BirthDate { get; init; }

        /// <summary>
        /// The roles assigned to the user.
        /// </summary>
        public IReadOnlyCollection<string> Roles { get; init; } = [];
    }
}
