using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Service
{
    public interface IContractorService
    {
        Task<Guid> CreateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default);

        Task<IEnumerable<Contractor>> GetContractorsAsync(CancellationToken cancellationToken = default);
    }
}
