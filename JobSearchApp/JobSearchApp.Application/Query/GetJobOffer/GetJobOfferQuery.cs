using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;

namespace JobSearchApp.Application.Query.GetJobOffer
{
    public class GetJobOfferQuery : IRequest<IEnumerable<JobOfferData>>
    {
        public GetJobOfferQuery() { }
    }
}
