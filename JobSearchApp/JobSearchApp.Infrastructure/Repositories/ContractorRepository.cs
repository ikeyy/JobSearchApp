using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Contractor;
using JobSearchApp.Domain.DTO.Customer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace JobSearchApp.Infrastructure.Repositories
{
    public class ContractorRepository: RepositoryBase<Contractor>, IContractorRepository
    {
        public ContractorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Contractor>> GetAllContractorsAsync(CancellationToken cancellationToken = default, bool trackChanges = false) =>
        await FindAll(trackChanges)
            .OrderBy(c => c.BusinessName)
            .ToListAsync(cancellationToken);

        public async Task<PagedResult<ContractorData>> SearchContractorAsync(
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

            IQueryable<Contractor> baseQuery = string.IsNullOrEmpty(query)
                                            ? FindAll()
                                            : FindByCondition(c =>
                                                c.BusinessName.StartsWith(query) ||
                                                (parsedId != Guid.Empty && c.Id == parsedId));

            var total = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(c => c.BusinessName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ContractorData
                {
                    ContractorId = c.Id,
                    BusinessName = c.BusinessName,
                    Rating = c.Rating
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<ContractorData>
            {
                Items = items,
                Total = total,
                Page = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Contractor?> GetContractorByIdAsync(Guid id, CancellationToken cancellationToken = default, bool trackChanges = false) =>
        await FindByCondition(c => c.Id == id, trackChanges)
            .SingleOrDefaultAsync(cancellationToken);

        public async Task<Contractor> CreateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default)
        {
            await _context.Contractor.AddAsync(contractor, cancellationToken);
            return contractor;
        }

        public async Task<Contractor> UpdateContractorAsync(Contractor contractor, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(contractor);

            var existing = await _context.Contractor
                .FindAsync(contractor.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Contractor with ID '{contractor.Id}' was not found.");

            existing.BusinessName = contractor.BusinessName;
            existing.Rating = contractor.Rating;
            existing.UpdatedAt = DateTime.Now;
            return existing;
        }

        public async Task<Contractor> DeleteContractorAsync(Guid contractorId, CancellationToken cancellationToken = default)
        {
            if (contractorId == Guid.Empty)
                throw new ArgumentException("Contractor ID must not be empty.", nameof(contractorId));

            var contractor = await _context.Contractor
                .FindAsync(contractorId, cancellationToken)
                ?? throw new KeyNotFoundException($"Contractor with ID '{contractorId}' was not found.");

            _context.Contractor.Remove(contractor);
            return contractor;
        }


    }
}
