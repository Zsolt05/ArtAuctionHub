using ArtAuctionHub.Application.Interfaces;

namespace ArtAuctionHub.Infrastructure.Services
{
    /// <summary>
    /// A mock implementation of IFavoriteService.
    /// </summary>
    public class FavoriteService : IFavoriteService
    {
        public void AddFavorite(int artworkId)
        {
            Console.WriteLine($"[FavoriteService] Added artwork {artworkId} to favorites.");
        }

        public void RemoveFavorite(int artworkId)
        {
            Console.WriteLine($"[FavoriteService] Removed artwork {artworkId} from favorites.");
        }

        public IEnumerable<string> GetFavorites()
        {
            return new[] { "Favorite1", "Favorite2" };
        }
    }
}