using ArtAuctionHub.Application.DTOs.Auth;
using ArtAuctionHub.Application.Interfaces;
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
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor for AuthController.
        /// </summary>
        /// <param name="authService">An implementation of IAuthService to handle authentication operations. Registered in the Dependency Injection (DI) container.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            // In a real-world scenario, we would:
            // 1. Validate the dto (e.g., ensure the email is unique and the password is strong).
            // 2. Hash the password before storing it.
            // 3. Save the new user to the database.
            // Currently, we simply return a success message.
            try
            {
                await _authService.RegisterAsync(dto);
            }
            catch (Exception ex)
            {
                // Handle exceptions such as user already exists, invalid data, etc.
                return BadRequest(new { Message = ex.Message });
            }
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
            // Future implementation would include:
            // 1. Validate the provided credentials.
            // 2. Check the password hash against the stored hash in the database.
            // 3. Generate a JWT token if authentication is successful.
            // Here, we return a dummy token for demonstration purposes.
            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                // Handle exceptions such as invalid credentials, user not found, etc.
                return Unauthorized(new { Message = ex.Message });
            }
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
            //Future implementation would include:
            // 1. Checking if the user is authenticated (e.g., via a JWT token).
            // 2. If authenticated, retrieve the user's details from the database.
            // 3. Return the user's information.
            var userInfo = _authService.GetCurrentUser(User);
            if (userInfo == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            return Ok(userInfo);
        }
    }
}