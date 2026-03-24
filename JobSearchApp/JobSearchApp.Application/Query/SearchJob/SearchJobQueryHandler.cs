using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Interfaces.Repository;
using MediatR;

namespace JobSearchApp.Application.Query.SearchJob
{
    public class SearchJobQueryHandler : IRequestHandler<SearchJobQuery, PagedResult<JobData>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchJobQueryHandler(
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<JobData>> Handle(SearchJobQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _unitOfWork.Jobs.SearchJobsAsync(
                request.Parameters,
                cancellationToken);
            return jobs;
        }
    }
}
