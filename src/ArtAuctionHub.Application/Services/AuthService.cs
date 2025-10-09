using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ArtAuctionHub.Application.Services
{
    /// <summary>
    /// Authentication & registration service built on ASP.NET Core Identity.
    /// </summary>
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IJwtTokenFactory _jwt;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for AuthService.
        /// </summary>
        /// <param name="userManager">User manager for identity operations.</param>
        /// <param name="roleManager">Role manager for identity operations.</param>
        /// <param name="jwt">JWT token factory.</param>
        /// <param name="logger">Logger for authentication actions.</param>
        /// <param name="mapper">AutoMapper instance for object mapping.</param>
        public AuthService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IJwtTokenFactory jwt,
            ILogger<AuthService> logger,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a new user with the specified role using ASP.NET Identity.
        /// </summary>
        public async Task RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Register attempt for user: {Email}", dto.Email);
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email {Email} already registered.", dto.Email);
                throw new InvalidOperationException("This email is already registered.");
            }

            var existingByName = await _userManager.FindByNameAsync(dto.UserName);
            if (existingByName != null)
            {
                _logger.LogWarning("Registration failed: Username {UserName} already taken.", dto.UserName);
                throw new InvalidOperationException("This username is already taken.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
            {
                _logger.LogWarning("Registration failed: Unknown role {Role}.", dto.Role);
                throw new InvalidOperationException($"Unknown role: {dto.Role}");
            }

            var user = _mapper.Map<User>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User creation failed for {Email}: {Errors}", dto.Email, errors);
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, dto.Role);
            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
        }

        /// <summary>
        /// Validates user credentials and issues JWT token.
        /// </summary>
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for user: {Email}", dto.Email);
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: Invalid email {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid)
            {
                _logger.LogWarning("Login failed: Invalid password for {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            (string token, DateTime expires) = _jwt.CreateToken(user.Id, user.UserName!, user.Email!, roles);

            _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
            return new AuthResultDto(token, expires);
        }

        /// <summary>
        /// Retrieves details of the currently authenticated user.
        /// </summary>
        public User GetCurrentUser(int userId)
        {
            var user = _userManager.Users
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    BirthDate = u.BirthDate,
                    Roles = u.Roles.Select(r => new UserRole
                    {
                        Role = new Role
                        {
                            Name = r.Role.Name,
                        }
                    }).ToList(),

                })
                .SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogWarning("GetCurrentUser failed: User ID {UserId} not found.", userId);
                throw new InvalidOperationException("User not found.");
            }
            return user;
        }
    }
}