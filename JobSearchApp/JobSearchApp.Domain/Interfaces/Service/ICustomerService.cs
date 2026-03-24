using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Service
{
    public interface ICustomerService
    {
        Task<Guid> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);

        Task<IEnumerable<CustomerData>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    }
}
