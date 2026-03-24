using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Query.GetJob
{
    public class GetJobQueryHandler : IRequestHandler<GetJobQuery, IEnumerable<JobData>>
    {
        private readonly IJobService _jobService;

        public GetJobQueryHandler(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IEnumerable<JobData>> Handle(GetJobQuery request, CancellationToken cancellationToken)
        {
            var result = await _jobService.GetJobsAsync(cancellationToken);

            var jobDataList = result.Select(job => new JobData
            {
                JobId = job.Id,
                StartDate = job.StartDate,
                DueDate = job.DueDate,
                Budget = job.Budget,
                Description = job.Description,
                AcceptedBy = job.AcceptedBy
            });

            return jobDataList;
        }
    }
}
