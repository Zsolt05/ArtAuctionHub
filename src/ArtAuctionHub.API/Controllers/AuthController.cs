using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The AuthController handles authentication-related requests.
    /// It exposes endpoints for user registration, login, and retrieving the current user information.
    /// </summary>
    /// <remarks>
    /// Constructor for AuthController.
    /// </remarks>
    /// <param name="authService">An implementation of IAuthService to handle authentication operations. Registered in the Dependency Injection (DI) container.</param>
    /// <param name="logger">The logger instance for logging authentication actions.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/auth")] // Base route for authentication-related endpoints. (/api/auth)
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor for AuthController.
        /// </summary>
        /// <param name="authService">An implementation of IAuthService to handle authentication operations. Registered in the Dependency Injection (DI) container.</param>
        /// <param name="logger">The logger instance for logging authentication actions.</param>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user based on the data provided in the request body.
        /// </summary>
        /// <param name="dto">
        /// A RegisterDto object containing the new user's registration details (username, email, password, etc.).
        /// </param>
        /// <returns>
        /// A 200 OK response with a success message (this is a placeholder response).
        /// A 400 Bad Request response if the registration fails (e.g., user already exists, invalid data).
        /// </returns>
        [HttpPost]
        [Route("register")] // Matches POST /api/auth/register.
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            _logger.LogInformation("Register attempt for user: {Email}", dto.Email);
            await _authService.RegisterAsync(dto);
            _logger.LogInformation("User registered successfully: {Email}", dto.Email);
            return Ok(new { Message = "User registered successfully." });
        }

        /// <summary>
        /// Authenticates a user by checking their login credentials (email and password).
        /// </summary>
        /// <param name="dto">
        /// A LoginDto object containing the user's email and password.
        /// </param>
        /// <returns>
        /// A 200 OK response with a JWT token placeholder (in a future implementation, this would be a real token).
        /// A 401 Unauthorized response if the login fails (e.g., invalid credentials).
        /// </returns>
        [HttpPost]
        [Route("login")] // Matches POST /api/auth/login.
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login attempt for user: {Email}", dto.Email);
            var token = await _authService.LoginAsync(dto);
            _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Retrieves information about the currently authenticated user.
        /// </summary>
        /// <returns>
        /// A 200 OK response with the username or null if no user is authenticated.
        /// </returns>
        [HttpGet]
        [Route("me")] // Matches GET /api/auth/me.
        [Authorize]
        public IActionResult Me()
        {
            var userInfo = _authService.GetCurrentUser(User);
            _logger.LogInformation("Current user info requested: {User}", userInfo?.Username ?? "Unknown");
            return userInfo == null ? throw new UnauthorizedAccessException("User is not authenticated.") : (IActionResult)Ok(userInfo);
        }
    }
}