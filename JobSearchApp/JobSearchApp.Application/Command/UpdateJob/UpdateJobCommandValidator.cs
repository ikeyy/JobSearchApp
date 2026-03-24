using FluentValidation;
using JobSearchApp.Domain.DTO.Job;

namespace JobSearchApp.Application.Command.UpdateJob
{
    public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
    {
        public UpdateJobCommandValidator()
        {

            RuleFor(x => x.JobId)
                .NotEmpty()
                .WithMessage("Job ID is required");

            RuleFor(x => x.JobData)
                .NotNull()
                .WithMessage("Job data is required")
                .SetValidator(new JobDataValidator());
        }
    }

    public class JobDataValidator : AbstractValidator<JobData>
    {
        public JobDataValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Start date must be today or in the future");

            RuleFor(x => x.DueDate)
                .NotEmpty()
                .WithMessage("Due date is required")
                .GreaterThan(x => x.StartDate)
                .WithMessage("Due date must be after the start date");

            RuleFor(x => x.Budget)
                .GreaterThan(0)
                .WithMessage("Budget must be greater than 0");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .Length(10, 5000)
                .WithMessage("Description must be between 10 and 5000 characters");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Status is required")
                .Must(status => new[] { "Open", "Offered", "Accepted", "Completed", "Cancelled" }
                    .Contains(status))
                .WithMessage("Status must be one of: Open, Offered, Accepted, Completed, Cancelled");
        }
    }
}