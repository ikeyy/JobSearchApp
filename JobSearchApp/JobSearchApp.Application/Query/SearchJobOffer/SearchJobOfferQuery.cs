using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;

namespace JobSearchApp.Application.Query.SearchJobOffer
{
    public class SearchJobOfferQuery : IRequest<PagedResult<JobOfferData>>
    {
        public string Filter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SearchJobOfferQuery(
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
