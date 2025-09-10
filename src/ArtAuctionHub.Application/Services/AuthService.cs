using System.Security.Claims;
using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// Authentication & registration service built on ASP.NET Core Identity.
    /// </summary>
    public sealed class AuthService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtTokenFactory jwt) : IAuthService
    {
        /// <summary>
        /// Registers a new user with the specified role using ASP.NET Identity.
        /// </summary>
        public async Task RegisterAsync(RegisterDto dto)
        {
            // check if email already exists
            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("This email is already registered.");

            // check if username already exists
            var existingByName = await userManager.FindByNameAsync(dto.UserName);
            if (existingByName != null)
                throw new InvalidOperationException("This username is already taken.");

            // check if role exists
            var roleExists = await roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
                throw new InvalidOperationException($"Unknown role: {dto.Role}");

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            await userManager.AddToRoleAsync(user, dto.Role);
        }

        /// <summary>
        /// Validates user credentials and issues JWT token.
        /// </summary>
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var valid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var roles = await userManager.GetRolesAsync(user);

            (string token, DateTime expires) = jwt.CreateToken(user.Id, user.UserName!, user.Email!, roles);

            return new AuthResultDto(token, expires);
        }

        /// <summary>
        /// Reads ClaimsPrincipal into a DTO of the current user.
        /// </summary>
        public CurrentUserDto GetCurrentUser(ClaimsPrincipal user)
        {
            if (user?.Identity is not { IsAuthenticated: true })
            {
                return new CurrentUserDto
                {
                    Username = "Anonymous",
                    Email = null,
                    Authenticated = false,
                    Roles = Array.Empty<string>()
                };
            }

            return new CurrentUserDto
            {
                Username = user.Identity!.Name ?? "(no-username)",
                Email = user.FindFirst(ClaimTypes.Email)?.Value,
                Authenticated = true,
                Roles = user.FindAll(ClaimTypes.Role).Select(r => r.Value).ToArray()
            };
        }
    }
}