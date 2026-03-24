using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Service
{
    public interface IJobOfferService
    {
        Task<IEnumerable<JobOffer>> GetJobOffersAsync(CancellationToken cancellationToken = default);
        Task<Guid> CreateJobOfferAsync(JobOffer jobOffer, CancellationToken cancellationToken = default);
        Task<JobOffer> GetJobOfferByIdAsync(Guid jobOfferId, CancellationToken cancellationToken = default);
        Task<Guid> UpdateJobOfferAsync(JobOffer jobOffer, CancellationToken cancellationToken = default);
        Task<Guid> DeleteJobOfferAsync(Guid jobOfferId, CancellationToken cancellationToken = default);
    }
}
