using System.Security.Claims;
using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// Provides user registration and authentication operations using repository and unit-of-work abstractions.
    /// <para>
    /// This implementation is persistence-agnostic and depends only on domain contracts,
    /// making it easy to unit test and swap infrastructure concerns.
    /// </para>
    /// <remarks>
    /// Password hashing is delegated to <see cref="PasswordHasherService"/>; token creation is
    /// intentionally omitted and replaced by a mocked string.
    /// </remarks>
    /// </summary>
    /// <param name="users">User repository.</param>
    /// <param name="roles">Role repository.</param>
    /// <param name="userRoles">User-role link repository.</param>
    /// <param name="uow">Unit of work for coordinating transactions and persistence.</param>
    public sealed class AuthService(
        IUserRepository users,
        IRoleRepository roles,
        IUserRoleRepository userRoles,
        IUnitOfWork uow) : IAuthService
    {
        /// <summary>
        /// Registers a new user and assigns the requested role atomically.
        /// </summary>
        /// <param name="dto">Incoming registration data.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when e-mail or username already exists, or the role does not exist.
        /// </exception>
        public async Task RegisterAsync(RegisterDto dto)
        {
            if (await users.ExistsByEmailAsync(dto.Email).ConfigureAwait(false))
                throw new InvalidOperationException("This email is already registered.");

            if (await users.ExistsByUsernameAsync(dto.UserName).ConfigureAwait(false))
                throw new InvalidOperationException("This username is already taken.");

            var role = await roles.GetByNameAsync(dto.Role).ConfigureAwait(false)
                       ?? throw new InvalidOperationException($"Unknown role: {dto.Role}");

            var user = new User
            {
                Username = dto.UserName,
                Email = dto.Email,
                PasswordHash = PasswordHasherService.HashPassword(dto.Password)
            };

            await uow.ExecuteInTransactionAsync(async () =>
            {
                await users.AddAsync(user).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false); // ensure Id

                // Avoid duplicate assignment if a unique constraint is not present yet.
                if (!await userRoles.HasRoleAsync(user.Id, role.Id).ConfigureAwait(false))
                {
                    await userRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id })
                                   .ConfigureAwait(false);
                }

                await uow.SaveChangesAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Validates user credentials and returns a mocked token.
        /// </summary>
        /// <param name="dto">Login data.</param>
        /// <returns>Mocked token string representing an authenticated session.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when credentials are invalid.</exception>
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await users.GetByEmailAsync(dto.Email).ConfigureAwait(false);

            if (user is null || !PasswordHasherService.VerifyPassword(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return "mocked-jwt-token";
        }

        /// <summary>
        /// Projects a minimal representation of the current principal.
        /// </summary>
        /// <param name="user">Current <see cref="ClaimsPrincipal"/>.</param>
        /// <returns>Anonymous DTO with <c>Username</c> and <c>Authenticated</c> fields.</returns>
        public object GetCurrentUser(ClaimsPrincipal user)
            => new
            {
                Username = user.Identity?.Name ?? "Anonymous",
                Authenticated = user.Identity?.IsAuthenticated ?? false
            };
    }
}