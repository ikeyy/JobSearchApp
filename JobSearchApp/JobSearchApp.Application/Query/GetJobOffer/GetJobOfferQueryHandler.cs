using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Query.GetJobOffer
{
    public class GetJobOfferQueryHandler : IRequestHandler<GetJobOfferQuery, IEnumerable<JobOfferData>>
    {
        private readonly IJobOfferService _jobOfferService;

        public GetJobOfferQueryHandler(IJobOfferService jobOfferService)
        {
            _jobOfferService = jobOfferService;
        }

        public async Task<IEnumerable<JobOfferData>> Handle(GetJobOfferQuery request, CancellationToken cancellationToken)
        {
            var result = await _jobOfferService.GetJobOffersAsync(cancellationToken);

            var jobOfferDataList = result.Select(jobOffer => new JobOfferData
            {
                JobOfferId = jobOffer.Id,
                JobId = jobOffer.JobId,
                ContractorId = jobOffer.ContractorId,
                Status = jobOffer.Status,
                Price = jobOffer.Price,
            });

            return jobOfferDataList;
        }
    }
}
