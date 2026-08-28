using ErrorOr;
using FluentValidation;
using MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr // Важно: для работы с ErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(); // Если валидаторов нет, пропускаем

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            // Собираем ошибки в список и возвращаем как ErrorOr
            var errors = failures.Select(f => Error.Validation(f.PropertyName, f.ErrorMessage)).ToList();

            // valueType — тип значения, которое ожидается в TResponse
            var valueType = typeof(TResponse).GetGenericArguments()[0];

            // Находим метод ToErrorOr (он находится в статическом классе ErrorOrExtensions)
            var toErrorOrMethod = typeof(ErrorOrExtensions)
                .GetMethod(nameof(ErrorOrExtensions.ToErrorOr))
                ?.MakeGenericMethod(valueType);

            // Вызываем метод, передавая список ошибок
            var result = toErrorOrMethod!.Invoke(null, new object[] { errors });

            // Приводим к TResponse и возвращаем
            return (TResponse)result!;
        }

        return await next();
    }
}