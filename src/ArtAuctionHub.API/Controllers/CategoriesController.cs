using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The CategoriesController provides a list of available artwork categories.
    /// </summary>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/categories")] // Base route for category-related endpoints. (/api/categories)
    public class CategoriesController : ControllerBase
    {
        /// <summary>
        /// Retrieves all available artwork categories.
        /// </summary>
        /// <returns>A 200 OK response with a list of categories.</returns>
        [HttpGet] // Matches GET /api/categories.
        public IActionResult GetCategories()
        {
            return Ok(new[] { "Painting", "Sculpture", "Digital Art" });
        }
    }
}