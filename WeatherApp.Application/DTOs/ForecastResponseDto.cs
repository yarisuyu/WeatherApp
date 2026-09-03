using System;
using System.Collections.Generic;

namespace WeatherApp.Application.DTOs
{
    public record ForecastResponseDto
    {
        public LocationDto Location { get; init; } = null!;
        public List<HourlyForecastDto> HourlyForecasts { get; init; } = new();
        public List<DailyForecastDto> DailyForecasts { get; init; } = new();
    }
}
