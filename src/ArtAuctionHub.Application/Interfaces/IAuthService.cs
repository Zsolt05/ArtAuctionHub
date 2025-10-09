using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Domain.Entities;

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
        /// <returns>A JWT token.</returns>
        Task<AuthResultDto> LoginAsync(LoginDto dto);

        /// <summary>
        /// Retrieves details of the currently authenticated user.
        /// </summary>
        /// <param name="userId">The ID of the current user.</param>
        /// <returns>The current user entity.</returns>
        User GetCurrentUser(int userId);
    }
}