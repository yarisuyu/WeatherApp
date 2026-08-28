namespace WeatherApp.Application.DTOs
{
    public record DailyForecastDto
    {
        public DateTime Date { get; init; }
        public double MaxTemperatureCelsius { get; init; }
        public double MinTemperatureCelsius { get; init; }
        public double AvgTemperatureCelsius { get; init; }
        public int ConditionCode { get; init; }
        public string ConditionText { get; init; } = string.Empty;
        public double TotalPrecipitationMm { get; init; }
    }

}
