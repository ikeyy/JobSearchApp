using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Contractor;
using JobSearchApp.Domain.Interfaces.Repository;
using MediatR;

namespace JobSearchApp.Application.Query.SearchContractor
{
    public class SearchContractorQueryHandler : IRequestHandler<SearchContractorQuery, PagedResult<ContractorData>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchContractorQueryHandler(
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ContractorData>> Handle(SearchContractorQuery request, CancellationToken cancellationToken)
        {
            var customers = await _unitOfWork.Contractors.SearchContractorAsync(
                request.Filter,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
            return customers;
        }
    }
}
