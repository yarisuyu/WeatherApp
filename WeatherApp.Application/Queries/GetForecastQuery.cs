using ErrorOr;
using MediatR;
using WeatherApp.Application.DTOs;

namespace WeatherApp.Application.Queries
{
    public record GetForecastQuery(double Latitude, double Longitude, int DayCount = 3)
    : IRequest<ErrorOr<ForecastResponseDto>>;
}
