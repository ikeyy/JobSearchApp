using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;

namespace JobSearchApp.Domain.Services
{
    public class JobService: IJobService
    {
        private readonly IUnitOfWork _unitOfWork;

        public JobService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateJobAsync(Job job, CancellationToken cancellationToken = default)
        {        
            await _unitOfWork.Jobs.CreateJobAsync(job, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return job.Id;
        }

        public async Task<IEnumerable<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            var jobs = await _unitOfWork.Jobs.GetAllJobsAsync(cancellationToken);

            return jobs;
        }

        public async Task<Job> GetJobByIdAsync(Guid jobId,CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Jobs.GetJobByIdAsync(jobId, cancellationToken);
        }

        public async Task<Guid> UpdateJobAsync(Job job, CancellationToken cancellationToken = default)
        {                
            _unitOfWork.Jobs.UpdateJob(job);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return job.Id;
        }

        public async Task<Guid> DeleteJobAsync(Guid jobId, CancellationToken cancellationToken = default)
        {
            var job = await _unitOfWork.Jobs.GetJobByIdAsync(jobId, cancellationToken, true);

            if(job == null)
                throw new KeyNotFoundException($"Job with id {jobId} not found.");

            await _unitOfWork.Jobs.DeleteJobAsync(jobId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return jobId;
        }
    }
}
