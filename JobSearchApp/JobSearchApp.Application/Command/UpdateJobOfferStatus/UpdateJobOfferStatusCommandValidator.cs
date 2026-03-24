using FluentValidation;
using JobSearchApp.Application.Command.AcceptJobOffer;
using JobSearchApp.Domain.Interfaces.Repository;

namespace JobSearchApp.Application.Command.UpdateJobOfferStatus
{
    public class UpdateJobOfferStatusCommandValidator : AbstractValidator<UpdateJobOfferStatusCommand>
    {
        private readonly IJobOfferRepository _jobOfferRepository;

        private readonly ICustomerRepository _customerRepository;

        public UpdateJobOfferStatusCommandValidator(
            IJobOfferRepository jobOfferRepository,
            ICustomerRepository customerRepository)
        {
            _jobOfferRepository = jobOfferRepository;
            _customerRepository = customerRepository;

            RuleFor(x => x.SetJobOfferStatus)
                .NotNull()
                .WithMessage("JobOfferStatus data is required");

            RuleFor(x => x.SetJobOfferStatus.JobOfferId)
                .NotEmpty()
                .WithMessage("JobOfferId is required")
                .Must(x => x != Guid.Empty)
                .WithMessage("JobOfferId must be a valid GUID")
                .MustAsync(JobOfferExists)
                .WithMessage("Job offer does not exist");

            RuleFor(x => x.SetJobOfferStatus.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required")
                .Must(x => x != Guid.Empty)
                .WithMessage("CustomerId must be a valid GUID")
                .MustAsync(CustomerExists)
                .WithMessage("Customer does not exist");

            RuleFor(x => x.SetJobOfferStatus.Status)
                .NotEmpty()
                .WithMessage("Status is required")
                .Must(status => new[] { "Pending", "Accepted", "Rejected" }
                    .Contains(status))
                .WithMessage("Status must be either Pending, Accepted, or Rejected");
        }

        private async Task<bool> JobOfferExists(Guid jobOfferId, CancellationToken cancellationToken)
        {
            var jobOffer = await _jobOfferRepository.GetJobOfferByIdAsync(jobOfferId, cancellationToken);
            return jobOffer != null;
        }

        private async Task<bool> CustomerExists(Guid customerId, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId, cancellationToken);
            return customer != null;
        }
    }
}