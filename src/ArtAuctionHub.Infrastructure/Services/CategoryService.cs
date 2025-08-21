using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// Category read service that retrieves data through a repository abstraction.
    /// This keeps the service decoupled from EF Core and the actual persistence.
    /// </summary>
    /// <remarks>
    /// Creates the service with a category repository dependency.
    /// </remarks>
    /// <param name="categories">The repository providing access to categories.</param>
    public class CategoryService(ICategoryRepository categories) : ICategoryService
    {
        /// <summary>
        /// Gets the list of category names in alphabetical order.
        /// Uses projection on the repository's queryable to avoid materializing
        /// unnecessary columns.
        /// </summary>
        /// <returns>A sequence of category names.</returns>
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            // Option A: use the repo's queryable for lightweight projection
            return await categories
                .AsQueryable()
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync();

            // Option B: or use the convenience method and project in-memory:
            // var all = await _categories.ListAllOrderedByNameAsync();
            // return all.Select(c => c.Name);
        }
    }
}