using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace JobSearchApp.Infrastructure.Repositories
{
    public class JobOfferRepository: RepositoryBase<JobOffer>, IJobOfferRepository
    {
        public JobOfferRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<JobOffer>> GetAllJobOfferAsync(CancellationToken cancellationToken = default, bool trackChanges = false) =>
        await FindAll(trackChanges)
            .OrderBy(c => c.Price)
            .ToListAsync(cancellationToken);

        public async Task<JobOffer?> GetJobOfferByIdAsync(Guid jobOfferId, CancellationToken cancellationToken = default, bool trackChanges = false)
        {
            return await FindByCondition(c => c.Id == jobOfferId, trackChanges)
                            .SingleOrDefaultAsync(cancellationToken)
                            ?? throw new KeyNotFoundException($"JobOffer with ID '{jobOfferId}' was not found."); ;
        }

        public async Task<PagedResult<JobOfferData>> SearchJobOffersAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page must be greater than 0.", nameof(pageNumber));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100.", nameof(pageSize));

            Guid.TryParse(query, out var parsedId);

            IQueryable<JobOffer> baseQuery = string.IsNullOrEmpty(query)
                                            ? FindAll()
                                            : FindByCondition(c =>
                                                (parsedId != Guid.Empty 
                                                && (c.JobId == parsedId 
                                                || c.ContractorId == parsedId)
                                                ));

            var total = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new JobOfferData
                {
                    JobOfferId = c.Id,
                    JobId = c.JobId,
                    ContractorId = c.Contractor.Id,
                    ContractorName = c.Contractor.BusinessName,
                    Price = c.Price,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<JobOfferData>
            {
                Items = items,
                Total = total,
                Page = pageNumber,
                PageSize = pageSize
            };

        }

        public async Task<JobOffer> CreateJobOfferAsync(JobOffer jobOffer, CancellationToken cancellationToken = default)
        {
            await _context.JobOffer.AddAsync(jobOffer, cancellationToken);
            return jobOffer;
        }

        public JobOffer UpdateJobOffer(JobOffer jobOffer)
        {
            _context.JobOffer.Update(jobOffer);
            return jobOffer;
        }

        public JobOffer DeleteJobOffer(JobOffer jobOffer, CancellationToken cancellationToken = default)
        {           
            _context.JobOffer.Remove(jobOffer);
            return jobOffer;
        }
    }
}
