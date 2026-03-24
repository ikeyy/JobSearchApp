using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;

namespace JobSearchApp.Domain.Services
{
    public class JobOfferService : IJobOfferService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobOfferService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateJobOfferAsync(JobOffer jobOffer, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.JobOffers.CreateJobOfferAsync(jobOffer, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return jobOffer.Id;
        }   

        public async Task<IEnumerable<JobOffer>> GetJobOffersAsync(CancellationToken cancellationToken = default)
        {
            var jobOffers = await _unitOfWork.JobOffers.GetAllJobOfferAsync(cancellationToken);
            return jobOffers;
        }

        public async Task<JobOffer> GetJobOfferByIdAsync(Guid jobOfferId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.JobOffers.GetJobOfferByIdAsync(jobOfferId, cancellationToken);
        }

        public async Task<Guid> UpdateJobOfferAsync(JobOffer jobOffer, CancellationToken cancellationToken = default)
        {
            _unitOfWork.JobOffers.UpdateJobOffer(jobOffer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return jobOffer.Id;
        }

        public async Task<Guid> DeleteJobOfferAsync(Guid jobOfferId, CancellationToken cancellationToken = default)
        {
            var jobOffer = await _unitOfWork.JobOffers.GetJobOfferByIdAsync(jobOfferId, cancellationToken, true);

            if (jobOffer == null)
                throw new KeyNotFoundException($"jobOffer with id {jobOfferId} not found.");

            _unitOfWork.JobOffers.DeleteJobOffer(jobOffer, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return jobOfferId;
        }
    }
}
