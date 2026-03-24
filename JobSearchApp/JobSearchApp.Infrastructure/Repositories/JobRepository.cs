using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace JobSearchApp.Infrastructure.Repositories
{
    public class JobRepository : RepositoryBase<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken cancellationToken = default, bool trackChanges = false) =>
        await FindAll(trackChanges)
            .OrderBy(c => c.DueDate)
            .ToListAsync(cancellationToken);

        public async Task<Job?> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default, bool trackChanges = false)
        {
            return await FindByCondition(c => c.Id == jobId, trackChanges)
                            .SingleOrDefaultAsync(cancellationToken) 
                            ?? throw new KeyNotFoundException($"Job with ID '{jobId}' was not found."); ;
        }

        public async Task<PagedResult<JobData>> SearchJobsAsync(
        JobSearchParams parameters,
        CancellationToken cancellationToken = default)
        {
            if (parameters.PageNumber < 1)
                throw new ArgumentException("Page must be greater than 0.", nameof(parameters.PageNumber));

            if (parameters.PageSize < 1 || parameters.PageSize > 100)
                throw new ArgumentException("Page size must be between 1 and 100.", nameof(parameters.PageSize));

            IQueryable<Job> baseQuery = string.IsNullOrEmpty(parameters.Description)
                                        && parameters.MinBudget is null or 0
                                        && parameters.MaxBudget is null or 0
                                        && string.IsNullOrEmpty(parameters.Status)
                                            ? FindAll()
                                            : FindByCondition(c =>
                                                c.Description.StartsWith(parameters.Description) 
                                               || (c.Budget >= parameters.MinBudget && c.Budget <= parameters.MaxBudget)
                                               || c.Status == parameters.Status);

            var total = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderByDescending(c => c.CreatedAt)
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(c => new JobData
                {
                    JobId = c.Id,
                    StartDate = c.StartDate,
                    DueDate = c.DueDate,
                    Budget = c.Budget,
                    Description = c.Description,
                    Status = c.Status,
                    AcceptedBy = c.AcceptedBy,
                    CustomerId = c.CreatedBy,
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<JobData>
            {
                Items = items,
                Total = total,
                Page = parameters.PageNumber,
                PageSize = parameters.PageSize
            };

        }

        public async Task<Job> CreateJobAsync(Job job, CancellationToken cancellationToken = default)
        {
            await _context.Job.AddAsync(job, cancellationToken);
            return job;
        }

        public Job UpdateJob(Job job)
        {
            _context.Job.Update(job);
            return job;
        }

        public async Task<Job> DeleteJobAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            var job = await GetJobByIdAsync(jobId,cancellationToken,true);
            _context.Job.Remove(job);
            return job;
        }
    }
}
