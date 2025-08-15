using ArtAuctionHub.Application.DTOs.Auth;
using System.Security.Claims;

namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Interface for authentication services.
    /// Provides methods for user registration, login, and retrieving current user information.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registers a new user with the provided registration data.
        /// </summary>
        /// <param name="dto">The registration data transfer object (DTO).</param>
        Task RegisterAsync(RegisterDto dto);

        /// <summary>
        /// Authenticates a user based on their login credentials.
        /// </summary>
        /// <param name="dto">The login data transfer object (DTO).</param>
        /// <returns>A JWT token or any other form of authentication token.</returns>
        Task<string> LoginAsync(LoginDto dto);

        /// <summary>
        /// Retrieves details of the currently authenticated user.
        /// </summary>
        /// <param name="user">The claims principal (authenticated user context).</param>
        /// <returns>Information about the current user.</returns>
        object GetCurrentUser(ClaimsPrincipal user);
    }
}