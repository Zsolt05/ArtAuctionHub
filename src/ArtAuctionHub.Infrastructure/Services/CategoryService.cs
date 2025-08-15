using ArtAuctionHub.Application.Interfaces;
using ArtAuctionHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A mock implementation of ICategoryService.
    /// </summary>
    public class CategoryService(ArtAuctionHubDbContext dbContext) : ICategoryService
    {
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await dbContext.Categories
                .Select(c => c.Name)
                .ToListAsync();
        }
    }
}