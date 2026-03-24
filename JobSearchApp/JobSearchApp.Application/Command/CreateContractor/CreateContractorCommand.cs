using JobSearchApp.Domain.DTO.Contractor;
using JobSearchApp.Domain.DTO.Generic;
using MediatR;

namespace JobSearchApp.Application.Command.CreateContractor
{
    public class CreateContractorCommand : IRequest<ApiResponse<Guid>>
    {
        public ContractorData ContractorData { get; set; }
        public CreateContractorCommand(ContractorData contractorData)
        {
            ContractorData = contractorData;
        }
    }
}
