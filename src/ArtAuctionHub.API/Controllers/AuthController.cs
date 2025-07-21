using ArtAuctionHub.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The AuthController handles authentication-related requests.
    /// It exposes endpoints for user registration, login, and retrieving the current user information.
    /// </summary>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/auth")] // Base route for authentication-related endpoints. (/api/auth)
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Registers a new user based on the data provided in the request body.
        /// </summary>
        /// <param name="dto">
        /// A RegisterDto object containing the new user's registration details (username, email, password, etc.).
        /// </param>
        /// <returns>
        /// A 200 OK response with a success message (this is a placeholder response).
        /// </returns>
        [HttpPost]
        [Route("register")] // Matches POST /api/auth/register.
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            // In a real-world scenario, we would:
            // 1. Validate the dto (e.g., ensure the email is unique and the password is strong).
            // 2. Hash the password before storing it.
            // 3. Save the new user to the database.
            // Currently, we simply return a success message.
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
        /// </returns>
        [HttpPost]
        [Route("login")] // Matches POST /api/auth/login.
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // Normally, we would:
            // 1. Validate the provided credentials.
            // 2. Check the password hash against the stored hash in the database.
            // 3. Generate a JWT token if authentication is successful.
            // Here, we return a dummy token for demonstration purposes.
            return Ok(new { Token = "jwt-token-placeholder" });
        }

        /// <summary>
        /// Retrieves information about the currently authenticated user.
        /// </summary>
        /// <returns>
        /// A 200 OK response with the username or null if no user is authenticated.
        /// </returns>
        [HttpGet]
        [Route("me")] // Matches GET /api/auth/me.
        public IActionResult Me()
        {
            // The User property (inherited from ControllerBase) provides access
            // to the current user’s identity based on authentication middleware (e.g., JWT claims).
            // If no user is authenticated, User.Identity?.Name will be null.
            return Ok(new { User = User.Identity?.Name });
        }
    }
}