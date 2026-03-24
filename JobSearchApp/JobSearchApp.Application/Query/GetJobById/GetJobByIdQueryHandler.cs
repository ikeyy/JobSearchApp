using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Query.GetJobById
{
    public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobData>
    {
        private readonly IJobService _jobService;

        public GetJobByIdQueryHandler(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<JobData> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
        {
            var job = await _jobService.GetJobByIdAsync(request.JobId,cancellationToken);

            var jobData = new JobData
            {
                JobId = job.Id,
                StartDate = job.StartDate,
                DueDate = job.DueDate,
                Budget = job.Budget,
                Description = job.Description,
                AcceptedBy = job.AcceptedBy
            };

            return jobData;
        }
    }
}
