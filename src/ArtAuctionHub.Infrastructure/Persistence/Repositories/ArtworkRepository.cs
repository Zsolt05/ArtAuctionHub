using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Entity Framework Core backed implementation of <see cref="IArtworkRepository"/>.
    /// Leverages the base <c>EfRepository&lt;T&gt;</c> for generic behavior and
    /// adds read-optimized queries with <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})"/>.
    /// </summary>
    /// <remarks>
    /// Initializes a new repository instance bound to the provided <see cref="ArtAuctionHubDbContext"/>.
    /// The repository shares the same context instance and therefore the same transaction boundary
    /// as other repositories resolved within the same request scope.
    /// </remarks>
    /// <param name="db">The EF Core database context.</param>
    internal sealed class ArtworkRepository(ArtAuctionHubDbContext db) : EfRepository<Artwork>(db), IArtworkRepository
    {

        /// <inheritdoc />
        public async Task<List<Artwork>> GetAllAsync(CancellationToken ct = default)
            => await _set.AsNoTracking().ToListAsync(ct);

        /// <inheritdoc />
        public async Task<List<Artwork>> GetByArtistIdAsync(int artistId, CancellationToken ct = default)
            => await _set.AsNoTracking()
                         .Where(a => a.ArtistId == artistId)
                         .ToListAsync(ct);

        /// <inheritdoc />
        public Task<Artwork?> FindByIdAsync(int id, CancellationToken ct = default)
            => _set.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

        /// <inheritdoc />
        public Task<bool> ExistsAsync(int artworkId, CancellationToken ct = default) =>
            _set.AsNoTracking().AnyAsync(a => a.Id == artworkId, ct);
    }
}