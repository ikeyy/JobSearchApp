using JobSearchApp.Domain.DTO.Generic;
using JobSearchApp.Domain.Interfaces.Service;
using MediatR;

namespace JobSearchApp.Application.Command.DeleteJobOffer
{
    public class DeleteJobOfferCommandHandler : IRequestHandler<DeleteJobOfferCommand, ApiResponse<Guid>>
    {
        private readonly IJobOfferService _jobOfferService;

        public DeleteJobOfferCommandHandler(IJobOfferService jobOfferService)
        {
            _jobOfferService = jobOfferService;
        }

        public async Task<ApiResponse<Guid>> Handle(DeleteJobOfferCommand request, CancellationToken cancellationToken)
        {
            var jobOfferId = await _jobOfferService.DeleteJobOfferAsync(request.JobOfferId, cancellationToken);

            return ApiResponse<Guid>.SuccessResponse(jobOfferId, "Job Offer has been deleted");
        }
    }
}
