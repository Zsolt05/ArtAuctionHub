using System.Linq.Expressions;

namespace ArtAuctionHub.Domain.Interfaces
{
    /// <summary>
    /// Generic repository interface for managing entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the entity, must be a class.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets an entity by its identifier.
        /// Returns <c>null</c> if the entity cannot be found.
        /// </summary>
        /// <param name="id">Identifier value (Guid is typical, but not enforced).</param>
        /// <param name="ct">Cancellation token.</param>
        Task<T?> GetByIdAsync(object id, CancellationToken ct = default);

        /// <summary>
        /// Returns a queryable source that can be further composed.
        /// Prefer using read-only operations over materializing the entire set.
        /// </summary>
        IQueryable<T> AsQueryable();

        /// <summary>
        /// Finds a single entity that matches the given predicate or returns <c>null</c>.
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        /// <summary>
        /// Returns all entities that match the given predicate.
        /// If <paramref name="predicate"/> is <c>null</c>, all entities are returned.
        /// </summary>
        Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        /// <summary>
        /// Determines whether any entity satisfies the predicate.
        /// If <paramref name="predicate"/> is <c>null</c>, checks if any entity exists.
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        /// <summary>
        /// Counts entities that satisfy the predicate.
        /// If <paramref name="predicate"/> is <c>null</c>, counts all entities.
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        /// <summary>
        /// Adds a new entity to the underlying store (not persisted until UnitOfWork.SaveChangesAsync).
        /// </summary>
        Task AddAsync(T entity, CancellationToken ct = default);

        /// <summary>
        /// Adds a range of entities to the underlying store (not persisted until UnitOfWork.SaveChangesAsync).
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        /// <summary>
        /// Updates an entity. Implementations should attach the entity if needed and mark it as modified.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Updates a range of entities.
        /// </summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Removes a range of entities.
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);
    }
}