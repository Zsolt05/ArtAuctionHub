using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="ICategoryRepository"/>.
    /// Extends <see cref="EfRepository{T}"/> to reuse the generic CRUD operations
    /// and adds category-specific queries optimized for the read scenarios
    /// used by the application layer.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the repository using the application's DbContext.
    /// </remarks>
    /// <param name="db">The database context that acts as the Unit of Work.</param>
    internal sealed class CategoryRepository(ArtAuctionHubDbContext db)
                : EfRepository<Category>(db), ICategoryRepository
    {

        /// <inheritdoc />
        public Task<List<Category>> ListAllOrderedByNameAsync(CancellationToken ct = default) =>
            _set.OrderBy(c => c.Name).ToListAsync(ct);

        /// <inheritdoc />
        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
            _set.AnyAsync(c => c.Name.ToLower() == name.ToLower(), ct);
    }
}