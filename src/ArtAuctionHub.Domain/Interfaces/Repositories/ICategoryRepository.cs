using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces;

namespace ArtAuctionHub.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository contract dedicated to the <see cref="Category"/> aggregate.
    /// Inherits the generic data-access operations from <see cref="IRepository{T}"/>,
    /// and can be extended with category-specific queries when needed.
    /// </summary>
    public interface ICategoryRepository : IRepository<Category>
    {
        /// <summary>
        /// Retrieves all categories ordered by their display name.
        /// This is a convenient method for common read scenarios.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of <see cref="Category"/> entities.</returns>
        Task<List<Category>> ListAllOrderedByNameAsync(CancellationToken ct = default);

        /// <summary>
        /// Checks whether a category with the given name already exists (case-insensitive).
        /// </summary>
        /// <param name="name">The category name to check.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns><c>true</c> if the name exists; otherwise <c>false</c>.</returns>
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    }
}