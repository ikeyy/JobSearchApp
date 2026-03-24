using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Customer;
using MediatR;

namespace JobSearchApp.Application.Query.SearchCustomer
{
    public class SearchCustomerQuery : IRequest<PagedResult<CustomerData>>
    {
        public string Filter { get; set; } = string.Empty;
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SearchCustomerQuery(
            string filter,
            int pageNumber,
            int pageSize) 
        {
            Filter = filter;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
