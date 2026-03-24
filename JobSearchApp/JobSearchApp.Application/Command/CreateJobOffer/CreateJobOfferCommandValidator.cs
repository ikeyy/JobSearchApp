using FluentValidation;
using JobSearchApp.Domain.Interfaces.Repository;

namespace JobSearchApp.Application.Command.CreateJobOffer
{
    public class CreateJobOfferCommandValidator : AbstractValidator<CreateJobOfferCommand>
    {
        private readonly IJobRepository _jobRepository;

        private readonly IContractorRepository _contractorRepository;

        public CreateJobOfferCommandValidator(
            IJobRepository jobRepository,
            IContractorRepository contractorRepository
            )
        {
            _jobRepository = jobRepository;
            _contractorRepository = contractorRepository;

            RuleFor(x => x.JobOfferData.JobId)
                .NotEmpty()
                .WithMessage("JobId by is required")
                .Must(x => x != Guid.Empty)
                .WithMessage("JobId by must be a valid GUID")
                .MustAsync(JobExists)
                .WithMessage("Job does not exist");

            RuleFor(x => x.JobOfferData.ContractorId)
                .NotEmpty()
                .WithMessage("ContractorId by is required")
                .Must(x => x != Guid.Empty)
                .WithMessage("ContractorId by must be a valid GUID")
                .MustAsync(ContractorExists)
                .WithMessage("Contractor does not exist"); ;

            RuleFor(x => x.JobOfferData.Price)
                .NotEmpty()
                .WithMessage("Price is required")
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0")
                .LessThanOrEqualTo(decimal.MaxValue)
                .WithMessage("Price exceeds maximum allowed value");

            //RuleFor(x => x.JobOfferData.Status)
            //.NotEmpty()
            //.WithMessage("Status is required")
            //.Must(status => new[] { "Pending", "Accepted", "Rejected" }
            //    .Contains(status))
            //.WithMessage("Status must be either Pending, Accepted, or Rejected");
        }

        private async Task<bool> JobExists(Guid jobId, CancellationToken cancellationToken)
        {
            var customer = await _jobRepository.GetJobByIdAsync(jobId, cancellationToken);
            return customer != null;
        }

        private async Task<bool> ContractorExists(Guid contractorId, CancellationToken cancellationToken)
        {
            var customer = await _contractorRepository.GetContractorByIdAsync(contractorId, cancellationToken);
            return customer != null;
        }
    }
}