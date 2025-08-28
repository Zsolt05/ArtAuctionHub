using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core based implementation of <see cref="IUserRoleRepository"/>.
    /// </summary>
    internal sealed class UserRoleRepository(ArtAuctionHubDbContext db) : EfRepository<UserRole>(db), IUserRoleRepository
    {
        /// <inheritdoc/>
        public Task<IEnumerable<string>> GetRoleNamesForUserAsync(int userId)
        {
            return _set
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role!.Name)
                .ToListAsync()
                .ContinueWith(t => (IEnumerable<string>)t.Result);
        }

        /// <inheritdoc/>
        public Task<bool> HasRoleAsync(int userId, int roleId, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
    }
}