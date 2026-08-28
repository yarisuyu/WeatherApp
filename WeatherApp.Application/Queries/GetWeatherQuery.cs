using ErrorOr;
using MediatR;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Queries
{
    public record GetWeatherQuery(double Latitude, double Longitude)
    : IRequest<ErrorOr<WeatherDashboardDto>>;
}
