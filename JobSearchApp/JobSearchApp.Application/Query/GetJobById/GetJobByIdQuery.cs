using JobSearchApp.Domain.DTO.Job;
using MediatR;

namespace JobSearchApp.Application.Query.GetJobById
{
    public class GetJobByIdQuery : IRequest<JobData>
    {
        public Guid JobId;

        public GetJobByIdQuery(Guid jobId) {
            JobId = jobId;
        }
    }
}
