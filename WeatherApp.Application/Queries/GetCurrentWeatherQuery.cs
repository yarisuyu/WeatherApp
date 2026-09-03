using ErrorOr;
using MediatR;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Queries
{
    public record GetCurrentWeatherQuery(double Latitude, double Longitude)
    : IRequest<ErrorOr<CurrentWeatherResponseDto>>;
}
