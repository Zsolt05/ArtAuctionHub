using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core backed implementation of <see cref="IUserFavoritesRepository"/>.
    /// </summary>
    internal sealed class UserFavoritesRepository(ArtAuctionHubDbContext db) : EfRepository<UserFavorites>(db), IFavoritesRepository
    {

        /// <inheritdoc />
        public Task<UserFavorites?> GetByUserAndArtworkAsync(int userId, int artworkId, CancellationToken ct = default) =>
            _set.FirstOrDefaultAsync(f => f.UserId == userId && f.ArtworkId == artworkId, ct);

        /// <inheritdoc />
        public Task<List<Artwork>> GetFavoriteArtworksAsync(int userId, CancellationToken ct = default) =>
            _set
               .AsNoTracking()
               .Where(f => f.UserId == userId)
               .OrderByDescending(f => f.AddedDate)
               .Include(f => f.Artwork)
               .Select(f => f.Artwork)
               .ToListAsync(ct);
    }
}