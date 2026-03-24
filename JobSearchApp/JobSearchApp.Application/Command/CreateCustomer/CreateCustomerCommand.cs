using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.DTO.Generic;
using MediatR;

namespace JobSearchApp.Application.Command.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<ApiResponse<Guid>>
    {
        public CustomerData CustomerData { get; set; }
        public CreateCustomerCommand(CustomerData customerData)
        {
            CustomerData = customerData;
        }
    }
}
