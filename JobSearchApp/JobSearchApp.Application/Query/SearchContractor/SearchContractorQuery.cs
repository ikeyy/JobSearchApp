using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Contractor;
using MediatR;

namespace JobSearchApp.Application.Query.SearchContractor
{
    public class SearchContractorQuery : IRequest<PagedResult<ContractorData>>
    {
        public string Filter { get; set; }
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SearchContractorQuery(
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
