using ArtAuctionHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtAuctionHub.API.Controllers
{
    /// <summary>
    /// The CategoriesController provides a list of available artwork categories.
    /// </summary>
    /// <remarks>
    /// Constructor for the CategoriesController.
    /// </remarks>
    /// <param name="categoryService">An instance of ICategoryService to handle category operations. Registered in the Dependency Injection (DI) container.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/categories")] // Base route for category-related endpoints. (/api/categories)
    [Authorize]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {

        /// <summary>
        /// Retrieves all available artwork categories.
        /// </summary>
        /// <returns>A 200 OK response with a list of categories.</returns>
        [HttpGet] // Matches GET /api/categories.
        public async Task<IActionResult> GetCategories()
        {
            var categories = await categoryService.GetCategoriesAsync();
            return Ok(categories); // Returns 200 OK with the list of categories.
        }
    }
}