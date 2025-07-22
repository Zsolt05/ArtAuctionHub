namespace ArtAuctionHub.Application.Interfaces
{
    /// <summary>
    /// Provides the contract for retrieving available artwork categories.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves a list of available artwork categories.
        /// </summary>
        /// <returns>A collection of category names.</returns>
        IEnumerable<string> GetCategories();
    }
}