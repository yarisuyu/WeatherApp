namespace WeatherApp.Application.DTOs
{
    public record CurrentWeatherDto
    {
        public DateTime LastUpdated { get; init; }
        public double TemperatureCelsius { get; init; }
        public int ConditionCode { get; init; }
        public string ConditionText { get; init; } = string.Empty;
        public double WindSpeedKph { get; init; }
        public int HumidityPercent { get; init; }
        public double FeelsLikeCelsius { get; init; }
        public double UvIndex { get; init; }
    }
}
