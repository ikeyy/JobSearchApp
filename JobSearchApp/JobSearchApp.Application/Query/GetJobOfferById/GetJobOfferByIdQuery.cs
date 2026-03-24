using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;

namespace JobSearchApp.Application.Query.GetJobOfferById
{
    public class GetJobOfferByIdQuery : IRequest<JobOfferData>
    {
        public Guid JobOfferId;

        public GetJobOfferByIdQuery(Guid jobOfferId)
        {
            JobOfferId = jobOfferId;
        }
    }
}
