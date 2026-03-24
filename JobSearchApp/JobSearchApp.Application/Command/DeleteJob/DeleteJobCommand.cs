using JobSearchApp.Domain.DTO.Generic;
using MediatR;

namespace JobSearchApp.Application.Command.DeleteJob
{
    public class DeleteJobCommand: IRequest<ApiResponse<Guid>>
    {
        public Guid JobId { get; set; }
        public DeleteJobCommand(Guid jobId)
        {
            JobId = jobId;
        }
    }
}
