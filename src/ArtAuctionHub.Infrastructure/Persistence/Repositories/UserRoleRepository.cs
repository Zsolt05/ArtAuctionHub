using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="IUserRoleRepository"/>.
    /// </summary>
    internal sealed class UserRoleRepository : EfRepository<UserRole>, IUserRoleRepository
    {
        private readonly ArtAuctionHubDbContext _db;
        public UserRoleRepository(ArtAuctionHubDbContext db) : base(db) => _db = db;

        /// <inheritdoc/>
        public Task<bool> HasRoleAsync(int userId, int roleId, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
    }
}