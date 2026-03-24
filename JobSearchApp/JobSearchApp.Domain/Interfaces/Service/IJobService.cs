using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Service
{
    public interface IJobService
    {
        Task<IEnumerable<Job>> GetJobsAsync(CancellationToken cancellationToken = default);

        Task<Guid> CreateJobAsync(Job job, CancellationToken cancellationToken = default);

        Task<Job> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default);

        Task<Guid> UpdateJobAsync(Job job, CancellationToken cancellationToken = default);

        Task<Guid> DeleteJobAsync(Guid jobId, CancellationToken cancellationToken = default);
    }
}
