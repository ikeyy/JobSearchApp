using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.CreateContractor
{
    public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand, ApiResponse<Guid>>
    {
        private readonly IContractorService _contractorService;

        public CreateContractorCommandHandler(IContractorService contractorService)
        {
            _contractorService = contractorService;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
        {
            var contractor = new Contractor
            {
                Id = Guid.NewGuid(),
                BusinessName = request.ContractorData.BusinessName,
                Rating = request.ContractorData.Rating,
                CreatedAt = DateTime.UtcNow,
            };

            var contractorId = await _contractorService.CreateContractorAsync(contractor, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(contractorId, "Contractor has been added");

        }
    }
}
