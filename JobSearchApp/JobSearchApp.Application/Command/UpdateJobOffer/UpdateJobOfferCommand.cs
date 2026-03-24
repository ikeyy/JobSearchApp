using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;

namespace JobSearchApp.Application.Command.UpdateJobOffer
{
    public class UpdateJobOfferCommand : IRequest<ApiResponse<Guid>>
    {
        public JobOfferData JobOfferData { get; set; }
        public UpdateJobOfferCommand(JobOfferData jobOfferData)
        {
            JobOfferData = jobOfferData;
        }
    }
}
