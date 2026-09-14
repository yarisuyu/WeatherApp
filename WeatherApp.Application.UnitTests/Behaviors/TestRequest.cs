using ErrorOr;
using MediatR;

namespace WeatherApp.Application.UnitTests.Behaviors;

/// <summary>
/// Искусственный запрос для тестирования pipeline behaviors.
/// </summary>
public record TestRequest(string Value = "test") : IRequest<ErrorOr<string>>;