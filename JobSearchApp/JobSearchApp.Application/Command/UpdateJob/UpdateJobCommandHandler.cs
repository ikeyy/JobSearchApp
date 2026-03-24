using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.UpdateJob
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, ApiResponse<Guid>>
    {
        private readonly IJobService _jobService;

        public UpdateJobCommandHandler(
            IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<ApiResponse<Guid>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobService.GetJobByIdAsync(request.JobId, cancellationToken);

            if (job is null)
                return ApiResponse<Guid>.FailureResponse("Job not found");

            job.StartDate = request.JobData.StartDate;
            job.DueDate = request.JobData.DueDate;
            job.Budget = request.JobData.Budget;
            job.Description = request.JobData.Description;
            job.UpdatedAt = DateTime.UtcNow;

            var jobId = await _jobService.UpdateJobAsync(job,cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(job.Id, "Job has been updated");
        }
    }
}
