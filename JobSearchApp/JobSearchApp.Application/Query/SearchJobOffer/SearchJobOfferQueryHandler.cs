using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Interfaces.Repository;
using MediatR;

namespace JobSearchApp.Application.Query.SearchJobOffer
{
    public class SearchJobOfferQueryHandler : IRequestHandler<SearchJobOfferQuery, PagedResult<JobOfferData>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SearchJobOfferQueryHandler(
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<JobOfferData>> Handle(SearchJobOfferQuery request, CancellationToken cancellationToken)
        {
            var jobOffers = await _unitOfWork.JobOffers.SearchJobOffersAsync(
                request.Filter,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
            return jobOffers;
        }
    }
}
