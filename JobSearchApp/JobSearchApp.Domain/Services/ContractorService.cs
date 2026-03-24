using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;

namespace JobSearchApp.Domain.Services
{
    public class ContractorService :IContractorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Contractors.CreateContractorAsync(contractor, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return contractor.Id;
        }

        public async Task<IEnumerable<Contractor>> GetContractorsAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Contractors.GetAllContractorsAsync(cancellationToken);
        }
    }
}
