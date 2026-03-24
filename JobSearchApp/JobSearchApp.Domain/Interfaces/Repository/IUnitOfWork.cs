namespace JobSearchApp.Domain.Interfaces.Repository
{
    /// <summary>
    /// Represents the Unit of Work pattern for coordinating repository operations
    /// and managing transaction boundaries.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Gets the customer repository.
        /// </summary>
        ICustomerRepository Customers { get; }

        /// <summary>
        /// Gets the job repository.
        /// </summary>
        IJobRepository Jobs { get; }

        /// <summary>
        /// Gets the job offer repository.
        /// </summary>
        IJobOfferRepository JobOffers { get; }

        /// <summary>
        /// Gets the contractor repository.
        /// </summary>
        IContractorRepository Contractors { get; }

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
