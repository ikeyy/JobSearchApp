using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.DeleteJob
{
    public class DeleteJobCommandHandler: IRequestHandler<DeleteJobCommand, ApiResponse<Guid>>
    {
        private readonly IJobService _jobService;

        public DeleteJobCommandHandler(IJobService jobService)
        {
            _jobService = jobService;
        }


        public async Task<ApiResponse<Guid>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        {
            var jobId = await _jobService.DeleteJobAsync(request.JobId, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(jobId, "Job has been deleted");
        }
    }
}
