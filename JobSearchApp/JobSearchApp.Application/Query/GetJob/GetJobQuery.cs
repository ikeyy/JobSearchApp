using JobSearchApp.Domain.DTO.Job;
using MediatR;

namespace JobSearchApp.Application.Query.GetJob
{
    public class GetJobQuery : IRequest<IEnumerable<JobData>>
    {
        public GetJobQuery() { }
    }
}
