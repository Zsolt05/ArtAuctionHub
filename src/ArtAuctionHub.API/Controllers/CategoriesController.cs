using ArtAuctionHub.Application.Interfaces;
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
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor for the CategoriesController.
        /// </summary>
        /// <param name="categoryService">An instance of ICategoryService to handle category operations. Registered in the Dependency Injection (DI) container.</param>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all available artwork categories.
        /// </summary>
        /// <returns>A 200 OK response with a list of categories.</returns>
        [HttpGet] // Matches GET /api/categories.
        public IActionResult GetCategories()
        {
            // Future implementation would involve:
            // 1. Fetching categories from the database.
            // 2. Returning the list of categories in a suitable format (e.g., list of CategoryDto).
            var categories = _categoryService.GetCategories();
            return Ok(categories); // Returns 200 OK with the list of categories.
        }
    }
}