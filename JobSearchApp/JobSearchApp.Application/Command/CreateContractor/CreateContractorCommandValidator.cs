using FluentValidation;

namespace JobSearchApp.Application.Command.CreateContractor
{
    public class CreateContractorCommandValidator : AbstractValidator<CreateContractorCommand>
    {
        public CreateContractorCommandValidator()
        {
            RuleFor(x => x.ContractorData.BusinessName)
                .NotEmpty()
                .WithMessage("Business Name is required")
                .Length(1, 100)
                .WithMessage("BusinessName must be between 1 and 100 characters");

            RuleFor(x => x.ContractorData.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 to 5");
        }
    }
}
