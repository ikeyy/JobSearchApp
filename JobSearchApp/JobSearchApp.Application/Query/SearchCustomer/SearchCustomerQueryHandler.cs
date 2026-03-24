using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Interfaces.Repository;
using MediatR;

namespace JobSearchApp.Application.Query.SearchCustomer
{
    public class SearchCustomerQueryHandler : IRequestHandler<SearchCustomerQuery, PagedResult<CustomerData>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchCustomerQueryHandler(
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<CustomerData>> Handle(SearchCustomerQuery request, CancellationToken cancellationToken)
        {
            var customers = await _unitOfWork.Customers.SearchCustomersAsync(
                request.Filter,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
            return customers;
        }
    }
}
