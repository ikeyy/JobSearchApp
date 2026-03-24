using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.DTO.Generic;
using MediatR;

namespace JobSearchApp.Application.Query.GetCustomer
{
    public class GetCustomerQuery : IRequest<List<CustomerData>>
    {
        public GetCustomerQuery()
        {
        }
    }
}
