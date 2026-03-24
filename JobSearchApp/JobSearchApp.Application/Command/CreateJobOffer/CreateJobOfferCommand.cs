using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using MediatR;

namespace JobSearchApp.Application.Command.CreateJobOffer
{
    public class CreateJobOfferCommand : IRequest<JobOfferData>
    {
        public JobOfferData JobOfferData { get; set; }
        public CreateJobOfferCommand(JobOfferData jobOfferData)
        {
            JobOfferData = jobOfferData;
        }
    }
}
