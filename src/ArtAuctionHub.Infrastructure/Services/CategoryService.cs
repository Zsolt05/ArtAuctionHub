using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Category read service that retrieves data through a repository abstraction.
    /// This keeps the service decoupled from EF Core and the actual persistence.
    /// </summary>
    /// <remarks>
    /// Creates the service with a category repository dependency and logger.
    /// </remarks>
    /// <param name="categories">The repository providing access to categories.</param>
    /// <param name="logger">The logger instance for logging category actions.</param>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly ILogger<CategoryService> _logger;

        /// <summary>
        /// Constructor for CategoryService.
        /// </summary>
        /// <param name="categories">Injected category repository.</param>
        /// <param name="logger">Logger for category actions.</param>
        public CategoryService(ICategoryRepository categories, ILogger<CategoryService> logger)
        {
            _categories = categories;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of category names in alphabetical order.
        /// Uses projection on the repository's queryable to avoid materializing
        /// unnecessary columns.
        /// </summary>
        /// <returns>A sequence of category names.</returns>
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            _logger.LogInformation("Retrieving all artwork categories");
            var result = await _categories
                .AsQueryable()
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync();

            _logger.LogInformation("Returned {Count} categories", result.Count);
            return result;
        }
    }
}