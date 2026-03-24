using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;

namespace JobSearchApp.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Customers.CreateCustomerAsync(customer, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return customer.Id;
        }

        public async Task<IEnumerable<CustomerData>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
        {
            var customers = await _unitOfWork.Customers.GetAllCustomersAsync(cancellationToken);

            return customers;
        }
    }
}
