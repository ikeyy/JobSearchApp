using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Enums;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.AcceptJobOffer
{
    public class UpdateJobOfferStatusCommandHandler : IRequestHandler<UpdateJobOfferStatusCommand, ApiResponse<Guid>>
    {
        private readonly IJobOfferService _jobOfferService;
        private readonly IJobService _jobService;

        public UpdateJobOfferStatusCommandHandler(
            IJobOfferService jobOfferService,
            IJobService jobService
            )
        {
            _jobOfferService = jobOfferService;
            _jobService = jobService;
        }


        public async Task<ApiResponse<Guid>> Handle(UpdateJobOfferStatusCommand request, CancellationToken cancellationToken)
        {
            var jobOffer = await _jobOfferService.GetJobOfferByIdAsync(request.SetJobOfferStatus.JobOfferId, cancellationToken);

            if (jobOffer is null)
                return ApiResponse<Guid>.FailureResponse("JobOffer not found");

            jobOffer.Status = request.SetJobOfferStatus.Status;
            jobOffer.UpdatedAt = DateTime.UtcNow;

            var jobOfferId = await _jobOfferService.UpdateJobOfferAsync(jobOffer, cancellationToken);

            var jobs = await _jobService.GetJobByIdAsync(jobOffer.JobId);

            if (jobs is null)
                return ApiResponse<Guid>.FailureResponse("Job not found");

            if (request.SetJobOfferStatus.Status == JobOfferStatus.Accepted.ToString()) {
                jobs.Status = request.SetJobOfferStatus.Status;
                jobs.AcceptedBy = request.SetJobOfferStatus.CustomerId;
                await _jobService.UpdateJobAsync(jobs, cancellationToken);
            }  

            return ApiResponse<Guid>.SuccessResponse(jobOfferId, "Job Offer has been updated");
        }

    }
}
