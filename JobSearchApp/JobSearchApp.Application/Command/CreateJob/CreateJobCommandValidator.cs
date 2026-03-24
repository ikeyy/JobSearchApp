using FluentValidation;
using JobSearchApp.Domain.Interfaces.Repository;

namespace JobSearchApp.Application.Command.CreateJob
{
    public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateJobCommandValidator(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;

            RuleFor(x => x.JobData.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Start date must be today or in the future");

            RuleFor(x => x.JobData.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required")
                .GreaterThan(x => x.JobData.StartDate)
                .WithMessage("Due date must be after the start date");

            RuleFor(x => x.JobData.Budget)
                .NotEmpty()
                .WithMessage("Budget is required")
                .GreaterThan(0)
                .WithMessage("Budget must be greater than 0");

            RuleFor(x => x.JobData.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .Length(10, 5000)
                .WithMessage("Description must be between 10 and 5000 characters");

            RuleFor(x => x.JobData.Status)
                .NotEmpty()
                .WithMessage("Status is required")
                .Must(status => new[] { "Open", "Offered", "Accepted", "Completed", "Cancelled" }
                    .Contains(status))
                .WithMessage("Status must be one of: Open, Offered, Accepted, Completed, Cancelled");

        }     
    }
}
