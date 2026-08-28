namespace WeatherApp.Application.DTOs
{
    public record WeatherDashboardDto
    {
        public LocationDto Location { get; init; } = null!;
        public CurrentWeatherDto CurrentWeather { get; init; } = null!;
        public List<HourlyForecastDto> HourlyForecasts { get; init; } = new();
        public List<DailyForecastDto> DailyForecasts { get; init; } = new();
    }
}
