using ArtAuctionHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ArtAuctionHub.Infrastructure.Persistence.Repositories
{
    internal class EfRepository<T>(ArtAuctionHubDbContext db) : IRepository<T> where T : class
    {
        protected readonly ArtAuctionHubDbContext _db = db;
        protected readonly DbSet<T> _set = db.Set<T>();

        public Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
            => _set.FindAsync([id], ct).AsTask();

        public IQueryable<T> AsQueryable() => _set.AsQueryable();

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _set.FirstOrDefaultAsync(predicate, ct);

        public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? await _set.ToListAsync(ct) : await _set.Where(predicate).ToListAsync(ct);

        public Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? _set.AnyAsync(ct) : _set.AnyAsync(predicate, ct);

        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? _set.CountAsync(ct) : _set.CountAsync(predicate, ct);

        public Task AddAsync(T entity, CancellationToken ct = default) => _set.AddAsync(entity, ct).AsTask();
        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) => _set.AddRangeAsync(entities, ct);

        public void Update(T entity) => _set.Update(entity);
        public void UpdateRange(IEnumerable<T> entities) => _set.UpdateRange(entities);

        public void Remove(T entity) => _set.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
    }
}