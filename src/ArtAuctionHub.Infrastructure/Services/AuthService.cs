using System.Security.Claims;
using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A simple implementation of IAuthService.
    /// In a future implementation, this service would generate real JWT token.
    /// </summary>
    public class AuthService(ArtAuctionHubDbContext dbContext) : IAuthService
    {
        private const int BcryptWorkFactor = 12;
        public async Task RegisterAsync(RegisterDto dto)
        {
            // Check if the user already exists
            if (await dbContext.Users.AnyAsync(u => u.Email == dto.Email))
                throw new InvalidOperationException("This email is already registered.");
            if (await dbContext.Users.AnyAsync(u => u.Username == dto.UserName))
                throw new InvalidOperationException("This username is already taken.");

            // Create a new user entity
            var user = new User
            {
                Username = dto.UserName,
                Email = dto.Email,
                // Hash the password using BCrypt
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: BcryptWorkFactor)
            };

            // Get the role from the database
            var role = await dbContext.Roles.SingleOrDefaultAsync(r => r.Name == dto.Role)
                       ?? throw new InvalidOperationException($"Unknown role: {dto.Role}");

            // Add the user to the database
            dbContext.Users.Add(user);
            // Save the user in the database
            await dbContext.SaveChangesAsync();
            // Add the user to the specified role
            dbContext.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            // Save the user role in the database
            await dbContext.SaveChangesAsync();
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Get the user by email from the database
            var user = await dbContext.Users
                .SingleOrDefaultAsync(u => u.Email == dto.Email);

            // Check if the user exists and verify the password
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

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