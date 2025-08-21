namespace ArtAuctionHub.Domain.Interfaces
{
    /// <summary>
    /// Coordinates transactional work across multiple repositories.
    /// In EF Core, a DbContext already acts as a unit of work. This abstraction
    /// is useful for testing, orchestration, and to avoid referencing EF Core in upper layers.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Persists all pending changes to the underlying store within the current unit of work.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Retrieves a repository instance bound to this unit of work.
        /// The repository shares the same underlying context/transaction.
        /// </summary>
        /// <typeparam name="T">Aggregate root type.</typeparam>
        IRepository<T> GetRepository<T>() where T : class;

        /// <summary>
        /// Begins a database transaction and executes the provided delegate inside it.
        /// The transaction is committed if the delegate completes successfully; otherwise it is rolled back.
        /// </summary>
        /// <param name="action">Asynchronous action to execute inside the transaction.</param>
        /// <param name="ct">Cancellation token.</param>
        Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
    }
}