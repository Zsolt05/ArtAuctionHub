using ArtAuctionHub.Domain.Entities;
using ArtAuctionHub.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// EF Core backed implementation of <see cref="IUserRepository"/>.
    /// </summary>
    internal sealed class UserRepository(ArtAuctionHubDbContext db) : EfRepository<User>(db), IUserRepository
    {

        /// <inheritdoc />
        public Task<bool> ExistsAsync(int userId, CancellationToken ct = default) =>
            _set.AsNoTracking().AnyAsync(u => u.Id == userId, ct);

        /// <inheritdoc/>
        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(u => u.Email == email, ct);

        /// <inheritdoc/>
        public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(u => u.UserName == username, ct);

        /// <inheritdoc/>
        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _set.FirstOrDefaultAsync(u => u.Email == email, ct);
    }
}