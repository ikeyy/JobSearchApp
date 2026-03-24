using JobSearchApp.Application.DTO.Generic;
using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.DTO.JobOffer;
using JobSearchApp.Domain.Entities;
using JobSearchApp.Domain.Interfaces.Repository;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.CreateJobOffer
{
    public class CreateJobOfferCommandHandler : IRequestHandler<CreateJobOfferCommand, JobOfferData>
    {
        private readonly IJobOfferService _jobOfferService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateJobOfferCommandHandler(
            IJobOfferService jobOfferService,
            IUnitOfWork unitOfWork
            )
        {
            _jobOfferService = jobOfferService;
            _unitOfWork = unitOfWork;
        }


        public async Task<JobOfferData> Handle(CreateJobOfferCommand request, CancellationToken cancellationToken)
        {
            var jobOffer = new JobOffer
            {
                Id = Guid.NewGuid(),
                JobId = request.JobOfferData.JobId,
                ContractorId = request.JobOfferData.ContractorId,
                Price = request.JobOfferData.Price,
                Status = request.JobOfferData.Status,
                CreatedAt = DateTime.UtcNow
            };

            var jobOfferId = await _jobOfferService.CreateJobOfferAsync(jobOffer, cancellationToken);

            var jobOfferData = await _jobOfferService.GetJobOfferByIdAsync(jobOfferId, cancellationToken);

            var jobOfferDataResult = new JobOfferData
            {
                JobOfferId = jobOfferData.Id,
                JobId = jobOfferData.JobId,
                ContractorId = jobOfferData.ContractorId,
                Price = jobOfferData.Price,
                Status = jobOfferData.Status,
                CreatedAt = jobOfferData.CreatedAt
            };

            return jobOfferDataResult;
        }
    }
}
