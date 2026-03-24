using FluentValidation;
using MediatR;

namespace JobSearchApp.Application.Behavior
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = new List<FluentValidation.Results.ValidationFailure>();

                // Validate sequentially to avoid concurrent use of scoped services (eg. DbContext)
                foreach (var validator in _validators)
                {
                    var result = await validator.ValidateAsync(context, cancellationToken);
                    if (result != null)
                        failures.AddRange(result.Errors.Where(e => e != null));

                    if (failures.Any())
                        break;
                }

                if (failures.Any())
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
