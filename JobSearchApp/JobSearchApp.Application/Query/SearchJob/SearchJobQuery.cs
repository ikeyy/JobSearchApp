using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Job;
using MediatR;

namespace JobSearchApp.Application.Query.SearchJob
{
    public class SearchJobQuery : IRequest<PagedResult<JobData>>
    {
        public JobSearchParams Parameters { get; }

        public SearchJobQuery(
            JobSearchParams parameters)
        {
            Parameters = parameters;
        }
    }
}
