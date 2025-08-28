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
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/auth")] // Base route for authentication-related endpoints. (/api/auth)
    public class AuthController(IAuthService authService) : ControllerBase
    {
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
            await authService.RegisterAsync(dto);
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
            var token = await authService.LoginAsync(dto);
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
            var userInfo = authService.GetCurrentUser(User);
            return userInfo == null ? throw new UnauthorizedAccessException("User is not authenticated.") : (IActionResult)Ok(userInfo);
        }
    }
}