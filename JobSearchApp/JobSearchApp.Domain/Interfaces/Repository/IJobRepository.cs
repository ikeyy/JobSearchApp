using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Job;
using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Repository
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllJobsAsync(CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<Job?> GetJobByIdAsync(Guid id, CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<PagedResult<JobData>> SearchJobsAsync(JobSearchParams parameters,CancellationToken cancellationToken = default);

        Task<Job> CreateJobAsync(Job job, CancellationToken cancellationToken = default);

        Job UpdateJob(Job job);

        Task<Job> DeleteJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    }
}
