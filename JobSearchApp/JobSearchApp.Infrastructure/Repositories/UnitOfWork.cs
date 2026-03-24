using JobSearchApp.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace JobSearchApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ICustomerRepository _customerRepository;
        private IJobRepository _jobRepository;
        private IJobOfferRepository _jobOfferRepository;
        private IContractorRepository _contractorRepository;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ICustomerRepository Customers =>
            _customerRepository = new CustomerRepository(_context);

        public IJobRepository Jobs =>
            _jobRepository = new JobRepository(_context);

        public IJobOfferRepository JobOffers =>
            _jobOfferRepository = new JobOfferRepository(_context);

        public IContractorRepository Contractors =>
            _contractorRepository = new ContractorRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _transaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_transaction is not null)
                    await _transaction.DisposeAsync()!;
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _transaction?.RollbackAsync(cancellationToken)!;
            }
            finally
            {
                if (_transaction is not null)
                    await _transaction.DisposeAsync()!;
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
                await _transaction.DisposeAsync();

            if (_context is not null)
                await _context.DisposeAsync();
        }
    }
}