using FluentValidation;

namespace JobSearchApp.Application.Command.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerData.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .Length(1, 100)
                .WithMessage("First name must be between 1 and 100 characters");

            RuleFor(x => x.CustomerData.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .Length(1, 100)
                .WithMessage("Last name must be between 1 and 100 characters");
        }
    }
}

