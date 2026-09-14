using ErrorOr;
using FluentValidation;
using MediatR;

namespace WeatherApp.Application.Common.Interfaces;

public class ValidationBehavior<TRequest, TValue>
    : IPipelineBehavior<TRequest, ErrorOr<TValue>>
    where TRequest : IRequest<ErrorOr<TValue>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<ErrorOr<TValue>> Handle(
        TRequest request,
        RequestHandlerDelegate<ErrorOr<TValue>> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            return failures
                .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
                .ToList();
        }

        return await next(cancellationToken);
    }
}