using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Query.GetJobOfferById
{
    public class GetJobOfferByIdQueryHandler : IRequestHandler<GetJobOfferByIdQuery, JobOfferData>
    {
        private readonly IJobOfferService _jobOfferService;

        public GetJobOfferByIdQueryHandler(IJobOfferService jobOfferService)
        {
            _jobOfferService = jobOfferService;
        }

        public async Task<JobOfferData> Handle(GetJobOfferByIdQuery request, CancellationToken cancellationToken)
        {
            var jobOffer = await _jobOfferService.GetJobOfferByIdAsync(request.JobOfferId, cancellationToken);

            var jobOfferData = new JobOfferData
            {
                JobOfferId = jobOffer.Id,
                JobId = jobOffer.JobId,
                ContractorId = jobOffer.ContractorId,
                Status = jobOffer.Status,
                Price = jobOffer.Price
            };

            return jobOfferData;
        }
    }
}
