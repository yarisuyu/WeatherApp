using ErrorOr;
using MediatR;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Queries
{
    public record GetWeatherQuery(double Latitude, double Longitude)
    : IRequest<ErrorOr<WeatherData>>;
}
