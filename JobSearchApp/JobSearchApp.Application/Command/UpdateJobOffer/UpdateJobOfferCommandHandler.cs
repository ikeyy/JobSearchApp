using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.UpdateJobOffer
{
    public class UpdateJobOfferCommandHandler : IRequestHandler<UpdateJobOfferCommand, ApiResponse<Guid>>
    {
        private readonly IJobOfferService _jobOfferService;

        public UpdateJobOfferCommandHandler(IJobOfferService jobOfferService)
        {
            _jobOfferService = jobOfferService;
        }


        public async Task<ApiResponse<Guid>> Handle(UpdateJobOfferCommand request, CancellationToken cancellationToken)
        {
            var jobOffer = await _jobOfferService.GetJobOfferByIdAsync(request.JobOfferData.JobOfferId, cancellationToken);

            if (jobOffer is null)
                return ApiResponse<Guid>.FailureResponse("JobOffer not found");

            jobOffer.JobId = request.JobOfferData.JobId;
            jobOffer.Status = request.JobOfferData.Status;
            jobOffer.Price = request.JobOfferData.Price;
            jobOffer.UpdatedAt = DateTime.UtcNow;

            var jobOfferId = await _jobOfferService.UpdateJobOfferAsync(jobOffer, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(jobOfferId, "Job Offer has been updated");
        }
    }
}
