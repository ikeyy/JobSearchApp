using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.Job;
using MediatR;

namespace JobSearchApp.Application.Command.CreateJob
{
    public class CreateJobCommand : IRequest<ApiResponse<Guid>>
    {
        public JobData JobData { get; set; }
        public CreateJobCommand(JobData jobData)
        {
            JobData = jobData;
        }
    }
}
