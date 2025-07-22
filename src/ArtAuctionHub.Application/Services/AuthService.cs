using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
using System.Security.Claims;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// A simple (mocked) implementation of IAuthService.
    /// In a future implementation, this service would interact with
    /// the database and generate JWT token.
    /// </summary>
    public class AuthService : IAuthService
    {
        public void Register(RegisterDto dto)
        {
            // Here, you would typically save the user to the database.
            // This mock just prints to the console.
            Console.WriteLine($"[AuthService] User registered: {dto.Email}");
        }

        public string Login(LoginDto dto)
        {
            // Normally, you'd validate the user and return a JWT token.
            return "mocked-jwt-token";
        }

        public object GetCurrentUser(ClaimsPrincipal user)
        {
            return new
            {
                Username = user.Identity?.Name ?? "Anonymous",
                Authenticated = user.Identity?.IsAuthenticated ?? false
            };
        }
    }
}