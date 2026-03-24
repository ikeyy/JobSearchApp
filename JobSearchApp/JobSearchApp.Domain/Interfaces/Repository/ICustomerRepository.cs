using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerByIdAsync(Guid id,CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<Customer> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);

        Task<PagedResult<CustomerData>> SearchCustomersAsync(
            string query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<CustomerData>> GetAllCustomersAsync(
                                            CancellationToken cancellationToken = default,
                                            int page = 1,
                                            int pageSize = 10,
                                            bool trackChanges = false);
    }
}
