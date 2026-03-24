using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, ApiResponse<Guid>>
    {
        private readonly IJobService _jobService;

        public CreateJobCommandHandler(IJobService jobService)
        {
            _jobService = jobService;
        }


        public async Task<ApiResponse<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var job = new Job
            {
                Id = Guid.NewGuid(),
                StartDate = request.JobData.StartDate,
                DueDate = request.JobData.DueDate,
                Budget = request.JobData.Budget,
                Description = request.JobData.Description,
                Status = request.JobData.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.JobData.CustomerId
            };

            var jobId = await _jobService.CreateJobAsync(job, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(jobId, "Job has been added");
        }
    }
}
