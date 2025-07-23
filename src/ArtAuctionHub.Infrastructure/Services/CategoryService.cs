using ArtAuctionHub.Application.Interfaces;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A mock implementation of ICategoryService.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        public IEnumerable<string> GetCategories()
        {
            return new[] { "Painting", "Sculpture", "Digital Art" };
        }
    }
}