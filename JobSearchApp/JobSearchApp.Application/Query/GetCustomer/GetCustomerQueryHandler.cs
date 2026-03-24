using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Query.GetCustomer
{
    public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, IEnumerable<CustomerData>>
    {
        private readonly ICustomerService _customerService;

        public GetCustomerQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IEnumerable<CustomerData>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerService.GetAllCustomersAsync(cancellationToken);

            return customers;
        }
    }
}
