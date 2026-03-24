using Azure;
using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace JobSearchApp.Infrastructure.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<CustomerData>> GetAllCustomersAsync(
                                            CancellationToken cancellationToken = default,
                                            int page = 1,
                                            int pageSize = 10,
                                            bool trackChanges = false) =>
        await FindAll(trackChanges)
            .OrderBy(c => c.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CustomerData
            {
                CustomerId = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName
            })
            .ToListAsync(cancellationToken);

    public async Task<PagedResult<CustomerData>> SearchCustomersAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {         
            if (pageNumber < 1)
                throw new ArgumentException("Page must be greater than 0.", nameof(pageNumber));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100.", nameof(pageSize));

            Guid.TryParse(query, out var parsedId);

            IQueryable<Customer> baseQuery = string.IsNullOrEmpty(query) 
                                            ? FindAll()
                                            : FindByCondition(c =>
                                                c.FirstName.StartsWith(query) ||
                                                c.LastName.StartsWith(query) ||
                                                (parsedId != Guid.Empty && c.Id == parsedId));

            var total = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(c => c.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerData
                {
                    CustomerId = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<CustomerData>
            {
                Items = items,
                Total = total,
                Page = pageNumber,
                PageSize = pageSize
            };
                
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default, bool trackChanges = false) =>
        await FindByCondition(c => c.Id == id, trackChanges)
            .SingleOrDefaultAsync(cancellationToken);

        public async Task<Customer> CreateCustomerAsync(Customer customer,CancellationToken cancellationToken = default)
        {
            await _context.Customer.AddAsync(customer,cancellationToken);
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(customer);

            var existing = await _context.Customer
                .FindAsync(customer.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Customer with ID '{customer.Id}' was not found.");

            existing.FirstName = customer.FirstName;
            existing.LastName = customer.LastName;
            return existing;
        }

        public async Task<Customer> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID must not be empty.", nameof(customerId));

            var customer = await _context.Customer
                .FindAsync(customerId, cancellationToken)
                ?? throw new KeyNotFoundException($"Customer with ID '{customerId}' was not found.");

            _context.Customer.Remove(customer);
            return customer;
        }
    }
}
