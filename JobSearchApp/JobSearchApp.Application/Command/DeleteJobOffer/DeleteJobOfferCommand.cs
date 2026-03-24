using JobSearchApp.Domain.DTO.Generic;
using MediatR;

namespace JobSearchApp.Application.Command.DeleteJobOffer
{
    public class DeleteJobOfferCommand : IRequest<ApiResponse<Guid>>
    {
        public Guid JobOfferId { get; set; }
        public DeleteJobOfferCommand(Guid jobOfferId)
        {
            JobOfferId = jobOfferId;
        }
    }
}
