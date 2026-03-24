using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Repository
{
    public interface IJobOfferRepository
    {
        Task<IEnumerable<JobOffer>> GetAllJobOfferAsync(CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<JobOffer?> GetJobOfferByIdAsync(Guid jobOfferId, CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<PagedResult<JobOfferData>> SearchJobOffersAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

        Task<JobOffer> CreateJobOfferAsync(JobOffer jobOffer, CancellationToken cancellationToken = default);

        JobOffer UpdateJobOffer(JobOffer jobOffer);

        JobOffer DeleteJobOffer(JobOffer jobOffer, CancellationToken cancellationToken = default);
    }
}
