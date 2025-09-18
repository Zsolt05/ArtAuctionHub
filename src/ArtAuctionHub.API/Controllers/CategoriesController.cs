using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Shared.Constants;
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
    /// <param name="logger">The logger instance for logging category actions.</param>
    /// <param name="cacheService">The cache service instance for caching category data.</param>
    [ApiController] // Indicates that this controller responds to web API requests.
    [Route("api/categories")] // Base route for category-related endpoints. (/api/categories)
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Constructor for CategoriesController.
        /// </summary>
        /// <param name="categoryService">Injected category service.</param>
        /// <param name="logger">Logger for category actions.</param>
        /// <param name="cacheService">Injected cache service.</param>
        public CategoriesController(ICategoryService categoryService,
            ILogger<CategoriesController> logger, ICacheService cacheService)
        {
            _categoryService = categoryService;
            _logger = logger;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Retrieves all available artwork categories.
        /// </summary>
        /// <returns>A 200 OK response with a list of categories.</returns>
        [HttpGet] // Matches GET /api/categories.
        public async Task<IActionResult> GetCategories()
        {
            _logger.LogInformation("Fetching all categories.");
            // Try to get the categories from the cache
            var categories = await _cacheService.GetOrCreateAsync(
                CacheKeyNames.AllCategories,
                () => _categoryService.GetCategoriesAsync(),
                absoluteExpireTime: TimeSpan.FromMinutes(5),
                slidingExpireTime: TimeSpan.FromMinutes(2)
            );
            return Ok(categories); // Returns 200 OK with the list of categories
        }
    }
}