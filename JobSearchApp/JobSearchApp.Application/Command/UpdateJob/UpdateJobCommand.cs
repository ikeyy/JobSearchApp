using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.Job;
using MediatR;

namespace JobSearchApp.Application.Command.UpdateJob
{
    public class UpdateJobCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid JobId { get; set; }
        public JobData JobData { get; set; }
        public UpdateJobCommand(
            Guid jobId,
            JobData jobData)
        {
            JobId = jobId;
            JobData = jobData;
        }
    }
}
