using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Contractor;
using JobSearchApp.Domain.Entities;

namespace JobSearchApp.Domain.Interfaces.Repository
{
    public interface IContractorRepository
    {
        Task<Contractor> CreateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default);

        Task<IEnumerable<Contractor>> GetAllContractorsAsync(CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<PagedResult<ContractorData>> SearchContractorAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

        Task<Contractor?> GetContractorByIdAsync(Guid id, CancellationToken cancellationToken = default, bool trackChanges = false);

        Task<Contractor> UpdateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default);

        Task<Contractor> DeleteContractorAsync(Guid contractorId, CancellationToken cancellationToken = default);
    }
}
