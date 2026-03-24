using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;

namespace JobSearchApp.Application.Command.AcceptJobOffer
{
    public class UpdateJobOfferStatusCommand : IRequest<ApiResponse<Guid>>
    {
        public SetJobOfferStatus SetJobOfferStatus { get; set; }

        public UpdateJobOfferStatusCommand(
            SetJobOfferStatus setJobOfferStatus)
        {
            SetJobOfferStatus = setJobOfferStatus;
        }
    }
}
