using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="IRoleRepository"/>.
    /// </summary>
    internal sealed class RoleRepository(ArtAuctionHubDbContext db) : EfRepository<Role>(db), IRoleRepository
    {

        /// <inheritdoc/>
        public Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
            => _set.FirstOrDefaultAsync(r => r.Name == name, ct);
    }
}