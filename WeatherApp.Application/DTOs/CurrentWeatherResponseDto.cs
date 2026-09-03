namespace WeatherApp.Application.DTOs
{
    public record CurrentWeatherResponseDto
    {
        public LocationDto Location { get; init; } = null!;
        public CurrentWeatherDto CurrentWeather { get; init; } = null!;
    }
}
