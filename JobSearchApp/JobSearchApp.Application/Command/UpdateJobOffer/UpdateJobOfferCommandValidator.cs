using FluentValidation;
using JobSearchApp.Domain.Interfaces.Repository;

namespace JobSearchApp.Application.Command.UpdateJobOffer
{
    public class UpdateJobOfferCommandValidator : AbstractValidator<UpdateJobOfferCommand>
    {
        private readonly IJobOfferRepository _jobOfferRepository;

        public UpdateJobOfferCommandValidator(
            IJobOfferRepository jobOfferRepository
            )
        {
            _jobOfferRepository = jobOfferRepository;

            RuleFor(x => x.JobOfferData)
                .NotNull()
                .WithMessage("JobOffer data is required");

            RuleFor(x => x.JobOfferData.JobOfferId)
                .NotEmpty()
                .NotEqual(Guid.Empty)
                .WithMessage("JobOfferId is required")
                .MustAsync(JobOfferExists)
                .WithMessage("JobOfferId does not exist");
        }

        private async Task<bool> JobOfferExists(Guid jobOfferId, CancellationToken cancellationToken)
        {
            var customer = await _jobOfferRepository.GetJobOfferByIdAsync(jobOfferId, cancellationToken);
            return customer != null;
        }
    }
}
