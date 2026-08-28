namespace WeatherApp.Application.DTOs
{
    public record HourlyForecastDto
    {
        public DateTime Time { get; init; }
        public double TemperatureCelsius { get; init; }
        public int ConditionCode { get; init; }
        public string ConditionText { get; init; } = string.Empty;
        public double WindSpeedKph { get; init; }
        public int HumidityPercent { get; init; }
    }
}
